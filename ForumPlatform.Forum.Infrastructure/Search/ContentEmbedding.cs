using Pgvector;

namespace ForumPlatform.Forum.Infrastructure.Search
{
	public enum EmbeddingContentType { Thread, Comment }

	public static class EmbeddingSettings
	{
		// Fixed in the database schema. Changing it needs a migration and a full re-embed.
		public const int Dimensions = 1024;
	}

	public class ContentEmbedding
	{
		public Guid Id { get; private set; } = Guid.NewGuid();
		public Guid ContentId { get; private set; }
		public EmbeddingContentType ContentType { get; private set; }
		public Vector Embedding { get; private set; } = null!;
		public string Model { get; private set; } = null!;
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

		private ContentEmbedding() { }

		public static ContentEmbedding Create(Guid contentId, EmbeddingContentType type, float[] values, string model)
			=> new()
			{ 
				ContentId = contentId,
				ContentType = type,
				Embedding = new Vector(values),
				Model = model,
			};

		public void Replace(float[] values, string model)
		{
			Embedding = new Vector(values);
			Model = model;
			CreatedAt = DateTime.UtcNow;
		}

	}
}
