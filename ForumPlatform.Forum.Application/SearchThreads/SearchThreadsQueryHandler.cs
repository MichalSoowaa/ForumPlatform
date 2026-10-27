using ForumPlatform.Forum.Application.Search;
using ForumPlatform.Forum.Infrastructure.Search;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.SearchThreads
{
	public class SearchThreadsQueryHandler : IRequestHandler<SearchThreadsQuery, List<ThreadSearchHit>>
	{
		private const int ResultLimit = 20;

		private readonly HybridThreadSearcher _searcher;
		private readonly ThreadHitLoader _loader;

		public SearchThreadsQueryHandler(HybridThreadSearcher searcher, ThreadHitLoader loader)
		{
			_searcher = searcher;
			_loader = loader;
		}

		public async Task<List<ThreadSearchHit>> Handle(SearchThreadsQuery request, CancellationToken ct)
		{
			var matches = await _searcher.SearchAsync(request.Text.Trim(), ResultLimit, ct);

			return await _loader.LoadAsync(matches, ct);
		}
	}
}
