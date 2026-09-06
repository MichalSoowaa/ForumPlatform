using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ForumPlatform.Forum.Domain.Entities;
using Th = ForumPlatform.Forum.Domain.Entities.Thread;

namespace ForumPlatform.Forum.Infrastructure.Persistance
{
	public class ThreadConfiguration : IEntityTypeConfiguration<Th>
	{
		public void Configure(EntityTypeBuilder<Th> builder)
		{
			builder.HasKey(t => t.Id);

			builder.Property(t => t.Title).IsRequired().HasMaxLength(300);
			builder.Property(t => t.Body).IsRequired();
			builder.Property(t => t.SubforumId).IsRequired();
			builder.Property(t => t.AuthorId).IsRequired();
			builder.Property(t => t.CommentCount).IsRequired().HasDefaultValue(0);
			builder.Property(t => t.LastActivityAt).IsRequired();
			builder.Property(t => t.CreatedAt).IsRequired();

			builder.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();

			// Real FK: Subforum lives in the same module/schema, so a proper relational contraint is appropriate
			// (unlike AuthorId, which crosses into Users).
			builder.HasOne<Subforum>()
				.WithMany()
				.HasForeignKey(t => t.SubforumId)
				.OnDelete(DeleteBehavior.Restrict); // Don't cascade-delete threads if a subforum is ever hard-deleted - force that to be a deliberate, explicit operation.

			// Supports "list threads in this subforum"
			builder.HasIndex(t => t.SubforumId);

			builder.HasIndex(t => t.LastActivityAt);
			builder.HasIndex(t => t.CreatedAt);

			builder.HasQueryFilter(t => !t.IsDeleted);

			builder.ToTable("Threads");
		}
	}
}
