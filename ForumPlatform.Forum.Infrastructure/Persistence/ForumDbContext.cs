using ForumPlatform.Forum.Domain.Entities;
using ForumPlatform.Forum.Infrastructure.Persistence;
using ForumPlatform.Forum.Infrastructure.Search;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Forum.Infrastructure.Persistance
{
	/// <summary>
	/// DbContext for the Forum module.
	/// Owns Subforum, Thread, Comment - all mapped to the "forum" schema.
	/// </summary>
	public class ForumDbContext : DbContext
	{
		public DbSet<Subforum> Subforums { get; set; } = null!;
		public DbSet<ForumPlatform.Forum.Domain.Entities.Thread> Threads { get; set; } = null!;
		public DbSet<Comment> Comments { get; set; } = null!;
		public DbSet<ContentEmbedding> ContentEmbeddings { get; set; } = null!;

		public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.HasPostgresExtension("vector");
			modelBuilder.ApplyConfiguration(new ContentEmbeddingConfiguration());
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(ForumDbContext).Assembly);
		}
	}
}
