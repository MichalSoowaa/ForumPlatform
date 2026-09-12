using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ForumPlatform.Users.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using ForumPlatform.Users.Domain.Entities;

namespace ForumPlatform.Users.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{	
			var connectionString = configuration.GetConnectionString("Postgres")
				?? throw new InvalidOperationException(
					"Missing ConnectionStrings:DefaultConnection. Set it with " +
					"`dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\"` " +
					"from the ForumPlatform.WebApi project folder — see README.");

			// Register the UserDbContext with the dependency injection container
			services.AddDbContext<UserDbContext>(options =>
				options.UseNpgsql(connectionString));

			// match validation with my own
			services.AddIdentityCore<User>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequiredLength = 8;
			})
				.AddEntityFrameworkStores<UserDbContext>();

			return services;
		}
	}
}
