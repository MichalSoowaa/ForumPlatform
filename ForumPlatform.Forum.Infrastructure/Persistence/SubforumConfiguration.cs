using Microsoft.EntityFrameworkCore;
using ForumPlatform.Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumPlatform.Forum.Infrastructure.Persistance
{
	public class SubforumConfiguration : IEntityTypeConfiguration<Subforum>
	{
		public void Configure(EntityTypeBuilder<Subforum> builder)
		{
			builder.HasKey(s => s.Id);

			builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
			builder.Property(s => s.Description).IsRequired().HasMaxLength(1000);

			builder.Property(s => s.Status)
				.IsRequired()
				.HasConversion<string>() // Stores enum as text.
				.HasMaxLength(20);

			builder.Property(s => s.CreatedByUserId).IsRequired();
			builder.Property(s => s.CreatedAt).IsRequired();

			builder.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();

			// Name must be unique among non-deleted, non-rejected subforums.
			builder.HasIndex(s => s.Name)
				.IsUnique()
				.HasFilter("\"IsDeleted\" = false AND \"Status\" <> 'Rejected'");

			// Speeds up the moderation queue query ("show me all Pending subforums").
			builder.HasIndex(s => s.Status);
			builder.HasQueryFilter(s => !s.IsDeleted);

			builder.ToTable("Subforums");
		}
	}
}
