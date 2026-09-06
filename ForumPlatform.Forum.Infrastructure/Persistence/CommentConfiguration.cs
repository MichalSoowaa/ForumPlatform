using ForumPlatform.Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumPlatform.Forum.Infrastructure.Persistance
{
	public class CommentConfiguration : IEntityTypeConfiguration<Comment>
	{
		public void Configure(EntityTypeBuilder<Comment> builder)
		{
			builder.HasKey(c => c.Id);

			builder.Property(c => c.Body).IsRequired();
			builder.Property(c => c.ThreadId).IsRequired();
			builder.Property(c => c.AuthorId).IsRequired();
			builder.Property(c => c.ParentCommentId).IsRequired(false); // Null for top-level comments.
			builder.Property(c => c.Depth).IsRequired();
			builder.Property(c => c.CreatedAt).IsRequired();

			builder.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();

			builder.HasOne<ForumPlatform.Forum.Domain.Entities.Thread>()
				.WithMany()
				.HasForeignKey(c => c.ThreadId)
				.OnDelete(DeleteBehavior.Restrict);

			// Self referencing FK for the parent comment.
			// Restrict: deleting a parent shouldn't silently cascade-delete its replies;
			// that's a business decision to handle explicitly later
			// (e.g. "delete comment" might reassign children to null instead).
			builder.HasOne<Comment>()
				.WithMany()
				.HasForeignKey(c => c.ParentCommentId)
				.OnDelete(DeleteBehavior.Restrict);

			// Supports "load all comments for this thread"
			builder.HasIndex(c => c.ThreadId);

			builder.HasIndex(c => c.ParentCommentId);

			builder.HasQueryFilter(c => !c.IsDeleted);

			builder.ToTable("Comments");
		}
	}
}
