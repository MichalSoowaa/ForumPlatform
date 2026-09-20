using ForumPlatform.Reputation.Infrastructure.Persistence;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire.PostgreSql;

namespace ForumPlatform.Reputation.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddReputationInfrastructure(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("Postgres")
			?? throw new InvalidOperationException("Missing ConnectionStrings:Postgres.");

			services.AddDbContext<ReputationDbContext>(options =>
				options.UseNpgsql(connectionString));

			// Hangfire, backed by the same Postgres instance - no Redis/RabbitMQ.
			// Uses its own set of tables (hangfire schema), separate from your domain schemas.
			services.AddHangfire(
				config => config.UsePostgreSqlStorage(
					options => options.UseNpgsqlConnection(connectionString)));

			services.AddHangfireServer();
			return services;
		}
	}
}
