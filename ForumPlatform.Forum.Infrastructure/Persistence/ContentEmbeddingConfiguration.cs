using ForumPlatform.Forum.Infrastructure.Search;
using ForumPlatform.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Forum.Infrastructure.Persistence
{
	public class ContentEmbeddingConfiguration : IEntityTypeConfiguration<ContentEmbedding>
	{
		public void Configure(EntityTypeBuilder<ContentEmbedding> builder)
		{
			builder.HasKey(e => e.Id);

			builder.Property(e => e.ContentType).IsRequired().HasConversion<string>().HasMaxLength(20);
			builder.Property(e => e.Embedding).IsRequired().HasColumnType($"vector({EmbeddingSettings.Dimensions})");
			builder.Property(e => e.Model).IsRequired().HasMaxLength(50);
			builder.Property(e => e.CreatedAt).IsRequired();

			// One embedding per piece of content, same idea as QualityAssessment.
			builder.HasIndex(e => new { e.ContentId, e.ContentType }).IsUnique();

			// Approximate nearest-neighbour index, using cosine distance (vectors are normalized).
			builder.HasIndex(e => e.Embedding).HasMethod("hnsw").HasOperators("vector_cosine_ops");

			builder.ToTable("ContentEmbeddings");
		}
	}
}
