using ForumPlatform.Forum.Application.Search;
using ForumPlatform.Forum.Infrastructure.Search;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.FindSimilarThreads
{
	public class FindSimilarThreadsQueryHandler : IRequestHandler<FindSimilarThreadsQuery, List<ThreadSearchHit>>
	{
		// A starting guess, not tuned: high on purpose, because this is a nudge shown to someone
		// who is mid-sentence, and a wrong suggestion is more annoying than a missed one.
		private const double DuplicateSimilarityThreshold = 0.75;
		private const int ResultLimit = 5;

		private readonly HybridThreadSearcher _searcher;
		private readonly ThreadHitLoader _loader;

		public FindSimilarThreadsQueryHandler(HybridThreadSearcher searcher, ThreadHitLoader loader)
		{
			_searcher = searcher;
			_loader = loader;
		}

		public async Task<List<ThreadSearchHit>> Handle(FindSimilarThreadsQuery request, CancellationToken ct)
		{
			var draft = ContentEmbeddingService.ThreadText(request.Title.Trim(), request.Body.Trim());

			var matches = await _searcher.FindSimilarAsync(
				draft, DuplicateSimilarityThreshold, ResultLimit, ct);

			return await _loader.LoadAsync(matches, ct);
		}
	}
}
