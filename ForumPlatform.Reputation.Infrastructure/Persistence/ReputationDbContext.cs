using Microsoft.EntityFrameworkCore;
using ForumPlatform.Reputation.Domain.Entities;

namespace ForumPlatform.Reputation.Infrastructure.Persistence
{
	public class ReputationDbContext : DbContext
	{
		public DbSet<QualityAssessment> QualityAssessments { get; set; } = null!;

		public ReputationDbContext(DbContextOptions<ReputationDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new QualityAssessmentConfiguration());

			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				if (entityType.GetSchema() == null)
				{
					entityType.SetSchema("reputation");
				}
			}
		}
	}
}
