using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ForumPlatform.Users.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;

namespace ForumPlatform.Users.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{	
			var connectionString = configuration.GetConnectionString("Postgres")
				?? throw new InvalidOperationException(
					"Missing ConnectionStrings:DefaultConnection. Set it with " +
					"`dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\"` " +
					"from the ForumPlatform.WebApi project folder — see README.");

			// Register the UserDbContext with the dependency injection container
			services.AddDbContext<UserDbContext>(options =>
				options.UseNpgsql(connectionString));

			services.AddIdentityCore<IdentityUser<Guid>>()
				.AddEntityFrameworkStores<UserDbContext>();

			return services;
		}
	}
}
