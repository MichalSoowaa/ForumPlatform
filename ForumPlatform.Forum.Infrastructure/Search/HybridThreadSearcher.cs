using ForumPlatform.Forum.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Infrastructure.Search
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="ThreadId"></param>
	/// <param name="Score"> Reciprocal-rank-fusion score (search) or similarity (duplicates)</param>
	/// <param name="Similarity">Cosine similarity, null when the thread matched by keyword only.</param>
	public record ThreadMatch(Guid ThreadId, double Score, double? Similarity);

	public class HybridThreadSearcher
	{
		// Language-agnostic: no stemming, so it works for Polish and English alike. The semantic
		// half compensates for word forms that plain matching misses.
		private const string FtsConfig = "simple";

		// Starting guesses, not tuned. Semantic hits below this similarity are dropped, otherwise a
		// nonsense query would still return its "nearest" threads. Tune with real queries.
		private const double MinSemanticSimilarity = 0.30;

		private const int CandidatePool = 50; // candidates taken from each side before fusing
		private const int RrfK = 60; // standard reciprocal-rank-fusion constant

		private readonly ForumDbContext _db;
		private readonly IEmbeddingGenerator _generator;
		private readonly ILogger<HybridThreadSearcher> _logger;

		public HybridThreadSearcher(ForumDbContext db, IEmbeddingGenerator generator, ILogger<HybridThreadSearcher> logger)
		{
			_db = db;
			_generator = generator;
			_logger = logger;
		}

		/// <summary>
		/// Global search: semantic candidates + keyword candidates, fused by rank.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="limit"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async Task<List<ThreadMatch>> SearchAsync(string text, int limit, CancellationToken ct)
		{
			var semantic = new List<Candidate>();

			if (_generator.IsConfigured)
			{
				try
				{
					var embedding = await _generator.EmbedQueryAsync(text, ct);
					semantic = await SemanticCandidatesAsync(embedding, MinSemanticSimilarity, CandidatePool, ct);
				}
				catch (Exception ex) when (!ct.IsCancellationRequested && 
					(ex is HttpRequestException or TaskCanceledException or InvalidOperationException))
				{
					// Search degrades to keyword-only instead of failing outright.
					_logger.LogWarning(ex, "Semantic search unavailable; falling back to keyword results only.");
				}
			}

			var keyword = await KeywordCandidatesAsync(text, CandidatePool, ct);

			return Fuse(semantic, keyword, limit);
		}

		/// <summary>
		/// Duplicate detection: only threads whose meaning is close to the draft. The draft is embedded as a
		/// document (not a query) because it is compared against stored documents, thread to thread.
		/// </summary>
		public async Task<List<ThreadMatch>> FindSimilarAsync(string draftText, double minSimilarity, int limit, CancellationToken ct)
		{
			if (!_generator.IsConfigured)
				return new List<ThreadMatch>();

			try
			{
				var embedding = (await _generator.EmbedDocumentsAsync(new[] { draftText }, ct))[0];
				var candidates = await SemanticCandidatesAsync(embedding, minSimilarity, limit, ct);
				return candidates.Select(c => new ThreadMatch(c.Id, c.Similarity, c.Similarity)).ToList();
			}
			catch (Exception ex) when (!ct.IsCancellationRequested &&
				(ex is HttpRequestException or TaskCanceledException or InvalidOperationException))
			{
				_logger.LogWarning(ex, "Duplicate check unavailable; skipping suggestions.");
				return new List<ThreadMatch>();
			}
		}

		private async Task<List<Candidate>> SemanticCandidatesAsync(float[] embedding, double minSimilarity, int take, CancellationToken ct)
		{
			var query = new Vector(embedding);

			// The join to Threads is what applies the soft-delete filter: embeddings of deleted
			// threads are never returned, even though their rows still exist.
			var rows = await _db.ContentEmbeddings
				.Where(e => e.ContentType == EmbeddingContentType.Thread)
				.Join(_db.Threads, e => e.ContentId, t => t.Id,
					(e, t) => new { t.Id, Distance = e.Embedding.CosineDistance(query) })
				.OrderBy(x => x.Distance)
				.Take(take)
				.ToListAsync(ct);

			return rows
				.Select(r => new Candidate(r.Id, 1.0 - r.Distance))
				.Where(c => c.Similarity >= minSimilarity)
				.ToList();
		}

		private async Task<List<Guid>> KeywordCandidatesAsync(string text, int take, CancellationToken ct)
		{
			return await _db.Threads
				.Where(t => EF.Functions.ToTsVector(FtsConfig, t.Title + " " + t.Body)
					.Matches(EF.Functions.WebSearchToTsQuery(FtsConfig, text)))
				.OrderByDescending(t => EF.Functions.ToTsVector(FtsConfig, t.Title + " " + t.Body)
					.Rank(EF.Functions.WebSearchToTsQuery(FtsConfig, text)))
				.Select(t => t.Id)
				.Take(take)
				.ToListAsync(ct);
		}

		private static List<ThreadMatch> Fuse(IReadOnlyList<Candidate> semantic, IReadOnlyList<Guid> keyword, int limit)
		{
			var scores = new Dictionary<Guid, double>();
			var similarity = semantic.ToDictionary(c => c.Id, c => c.Similarity);

			for (var i = 0; i < semantic.Count; i++)
				scores[semantic[i].Id] = scores.GetValueOrDefault(semantic[i].Id) + 1.0 / (RrfK + i + 1);

			for (var i = 0; i < keyword.Count; i++)
				scores[keyword[i]] = scores.GetValueOrDefault(keyword[i]) + 1.0 / (RrfK + i + 1);

			return scores
				.OrderByDescending(kv => kv.Value)
				.ThenBy(kv => kv.Key)
				.Take(limit)
				.Select(kv => new ThreadMatch(kv.Key, kv.Value, similarity.TryGetValue(kv.Key, out var s) ? s : (double?)null))
				.ToList();
		}

		private record Candidate(Guid Id, double Similarity);
	}	
}
