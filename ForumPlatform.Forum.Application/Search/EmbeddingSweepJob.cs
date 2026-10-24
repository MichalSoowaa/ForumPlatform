using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Forum.Infrastructure.Search;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Search
{
	public class EmbeddingSweepJob
	{
		private const int BatchSize = 32;
		private const int MaxBatchesPerRun = 20;

		private readonly ForumDbContext _db;
		private readonly IEmbeddingGenerator _generator;
		private readonly ContentEmbeddingService _service;

		public EmbeddingSweepJob(ForumDbContext db, IEmbeddingGenerator generator, ContentEmbeddingService service)
		{
			_db = db;
			_generator = generator;
			_service = service;
		}

		[DisableConcurrentExecution(600)]
		public async Task Run()
		{
			if (!_generator.IsConfigured)
				return;

			for (var batch = 0; batch < MaxBatchesPerRun; batch++)
			{
				var threads = await _db.Threads
					.Where(t => !_db.ContentEmbeddings.Any(e => e.ContentId == t.Id && e.ContentType == EmbeddingContentType.Thread))
					.OrderBy(t => t.CreatedAt)
					.Take(BatchSize)
					.Select(t => new { t.Id, t.Title, t.Body })
					.ToListAsync();

				var comments = await _db.Comments
				.Where(c => !_db.ContentEmbeddings.Any(
					e => e.ContentId == c.Id && e.ContentType == EmbeddingContentType.Comment))
				.OrderBy(c => c.CreatedAt)
				.Take(BatchSize)
				.Select(c => new { c.Id, c.Body })
				.ToListAsync();

				var inputs = threads
					.Select(t => new EmbeddingInput(t.Id, EmbeddingContentType.Thread, ContentEmbeddingService.ThreadText(t.Title, t.Body)))
					.Concat(comments.Select(c => new EmbeddingInput(c.Id, EmbeddingContentType.Comment, c.Body)))
					.ToList();

				if (inputs.Count == 0) return;

				await _service.StoreAsync(inputs, CancellationToken.None);
			}
		}
	}
}
