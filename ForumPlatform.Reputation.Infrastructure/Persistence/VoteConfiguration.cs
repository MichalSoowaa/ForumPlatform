using Microsoft.EntityFrameworkCore;
using ForumPlatform.Reputation.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumPlatform.Reputation.Infrastructure.Persistence
{
	public class VoteConfiguration : IEntityTypeConfiguration<Vote>
	{
		public void Configure(EntityTypeBuilder<Vote> builder)
		{
			builder.HasKey(v => v.Id);

			builder.Property(v => v.VoterId).IsRequired();
			builder.Property(v => v.ContentId).IsRequired();

			builder.Property(v => v.ContentType)
				.IsRequired()
				.HasConversion<string>()
				.HasMaxLength(20);

			builder.Property(v => v.Value)
				.IsRequired()
				.HasConversion<string>()
				.HasMaxLength(20);

			builder.Property(v => v.WeightAtCast).HasPrecision(5, 4);

			builder.Property(v => v.CreatedAt).IsRequired();

			builder.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();

			// The actual invariant: one vote per (voter, content) pair.
			builder.HasIndex(v => new { v.VoterId, v.ContentId, v.ContentType }).IsUnique();

			// Supports "sum up all votes for this content" — the primary read pattern
			// for recomputing a community score.
			builder.HasIndex(v => new { v.ContentId, v.ContentType });

			builder.HasQueryFilter(v => !v.IsDeleted);

			builder.ToTable("Votes");
		}
	}
}
