using ForumPlatform.Reputation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumPlatform.Reputation.Infrastructure.Persistence
{
	public class QualityAssessmentConfiguration : IEntityTypeConfiguration<QualityAssessment>
	{
		public void Configure(EntityTypeBuilder<QualityAssessment> builder)
		{
			builder.ToTable("QualityAssessments");

			builder.HasKey(q => q.Id);

			builder.Property(q => q.ContentType)
			.IsRequired()
			.HasConversion<string>()
			.HasMaxLength(20);

			builder.Property(q => q.Status)
				.IsRequired()
				.HasConversion<string>()
				.HasMaxLength(20);

			builder.Property(q => q.HeuristicScore).HasPrecision(5, 4);
			builder.Property(q => q.LlmVerdict).HasPrecision(5, 4);

			builder.Property(q => q.CreatedAt).IsRequired();

			// One assessment per piece of content — this is what "attached to"
			// means concretely: a lookup by (ContentId, ContentType), not a list.
			builder.HasIndex(q => new { q.ContentId, q.ContentType }).IsUnique();
		}
	}
}
