using Microsoft.EntityFrameworkCore;
using ForumPlatform.Users.Domain.Entities.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumPlatform.Users.Infrastructure.Persistance
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(u => u.Id);
			builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
			builder.Property(u => u.UserName).IsRequired().HasMaxLength(256);
			builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100);
			builder.Property(u => u.TotalReputation).HasPrecision(18, 2);
			builder.Property(u => u.MeritIndicator).HasPrecision(5, 4);
			builder.Property(u => u.LastActiveAt).IsRequired();
			builder.Property(u => u.CreatedAt).IsRequired();
			builder.Property(u => u.UpdatedAt);
			builder.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);
			builder.Property(u => u.Version).IsRowVersion(); // Concurrency token for optimistic concurrency control

			builder.HasIndex(u => u.Email).IsUnique().HasFilter("\"IsDeleted\" = false"); // Unique index on Email for non-deleted users
			builder.HasIndex(u => u.UserName).IsUnique().HasFilter("\"IsDeleted\" = false");

			builder.HasQueryFilter(u => !u.IsDeleted); // Global query filter to exclude soft-deleted users

			builder.ToTable("Users"); // Specify the table name in the database
		}
	}
}
