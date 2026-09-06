using ForumPlatform.Forum.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddForumInfrastructure
			(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("Postgres")
				?? throw new InvalidOperationException("Missing ConnectionStrings: Postgres");

			services.AddDbContext<ForumDbContext>(options =>
				options.UseNpgsql(connectionString));

			return services;
		}
	}
}
