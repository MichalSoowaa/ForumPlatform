using Microsoft.EntityFrameworkCore;
using ForumPlatform.Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ForumThread = ForumPlatform.Forum.Domain.Entities.Thread;

namespace ForumPlatform.Forum.Infrastructure.Persistence
{
	public class ThreadVisitConfiguration : IEntityTypeConfiguration<ThreadVisit>
	{
		public void Configure(EntityTypeBuilder<ThreadVisit> builder)
		{
			builder.HasKey(v => v.Id);

			builder.Property(v => v.UserId).IsRequired();
			builder.Property(v => v.ThreadId).IsRequired();
			builder.Property(v => v.LastVisitedAt).IsRequired();
			builder.Property(v => v.SessionBaselineAt).IsRequired();
			builder.Property(v => v.CreatedAt).IsRequired();

			// One row per (user, thread). It also makes two simultaneous first visits safe.
			builder.HasIndex(v => new { v.UserId, v.ThreadId }).IsUnique();

			builder.HasOne<ForumThread>()
				.WithMany()
				.HasForeignKey(v => v.ThreadId)
				.OnDelete(DeleteBehavior.Cascade); // derived data: it goes away with its thread

			builder.ToTable("ThreadVisits");
		}
	}
}
