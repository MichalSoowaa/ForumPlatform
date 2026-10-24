using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Forum.Infrastructure.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Search
{
	public record EmbeddingInput(Guid ContentId, EmbeddingContentType ContentType, string Text);

	public class ContentEmbeddingService
	{
		private readonly ForumDbContext _db;
		private readonly IEmbeddingGenerator _generator;

		public ContentEmbeddingService(ForumDbContext db, IEmbeddingGenerator generator)
		{
			_db = db;
			_generator = generator;
		}

		// The thread's title is part of what it is "about", so it's embedded with the body.
		public static string ThreadText(string title, string body) => $"{title}\n\n{body}";

		public async Task StoreAsync(IReadOnlyList<EmbeddingInput> inputs, CancellationToken ct)
		{
			if (inputs.Count == 0) return;

			var vectors = await _generator.EmbedDocumentsAsync(inputs.Select(i => i.Text).ToList(), ct);

			for (var i = 0; i < inputs.Count; i++)
			{
				var input = inputs[i];

				var existing = await _db.ContentEmbeddings.FirstOrDefaultAsync(
					e => e.ContentId == input.ContentId && e.ContentType == input.ContentType, ct);

				if (existing is null)
					_db.ContentEmbeddings.Add(
						ContentEmbedding.Create(input.ContentId, input.ContentType, vectors[i], _generator.ModelName));

				else
					existing.Replace(vectors[i], _generator.ModelName);
			}

			await _db.SaveChangesAsync(ct);
		}
	}
}
