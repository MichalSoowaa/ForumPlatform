using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Infrastructure.Search
{
	public interface IEmbeddingGenerator
	{
		bool IsConfigured { get; }
		string ModelName { get; }

		/// <summary>For text being stored (threads, comments).</summary>
		Task<IReadOnlyList<float[]>> EmbedDocumentsAsync(IReadOnlyList<string> texts, CancellationToken ct);

		/// <summary>For text being searched with (search box, draft thread).</summary>
		Task<float[]> EmbedQueryAsync(string text, CancellationToken ct);
	}
}
