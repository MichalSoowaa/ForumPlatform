using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Forum.Infrastructure.Search;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Search
{
	/// <summary>Embeds one new thread or comment right after it is created.</summary>
	public class EmbedContentJob
	{
		private readonly ForumDbContext _db;
		private readonly IEmbeddingGenerator _generator;
		private readonly ContentEmbeddingService _service;

		public EmbedContentJob(ForumDbContext db, IEmbeddingGenerator generator, ContentEmbeddingService service)
		{
			_db = db;
			_generator = generator;
			_service = service;
		}

		[AutomaticRetry(Attempts = 5)]
		public async Task Handle(Guid contentId, EmbeddingContentType contentType)
		{
			// Search is optional until a key is configured; the sweep job picks this up later.
			if (!_generator.IsConfigured)
				return;

			string? text;

			if (contentType == EmbeddingContentType.Thread)
			{
				var thread = await _db.Threads
					.Where(t => t.Id == contentId)
					.Select(t => new { t.Title, t.Body })
					.FirstOrDefaultAsync();

				text = thread is null ? null : ContentEmbeddingService.ThreadText(thread.Title, thread.Body);
			}
			else
			{
				text = await _db.Comments
					.Where(c => c.Id == contentId)
					.Select(c => c.Body)
					.FirstOrDefaultAsync();
			}

			if (text is null)
				return;

			await _service.StoreAsync(new[] { new EmbeddingInput(contentId, contentType, text) }, CancellationToken.None);
		}
	}
}
