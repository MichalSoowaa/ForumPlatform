using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Forum.Infrastructure.Search;
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
				options.UseNpgsql(connectionString, npgsql => npgsql.UseVector()));

			var voyage = new VoyageOptions
			{
				ApiKey = configuration["Voyage:ApiKey"] ?? "",
				Model = configuration["Voyage:Model"] ?? "voyage-4",
				BaseUrl = configuration["Voyage:BaseUrl"] ?? "https://ai.mongodb.com/v1",
			};

			services.AddSingleton(voyage);
			services.AddHttpClient<IEmbeddingGenerator, VoyageEmbeddingGenerator>(client =>
			{
				client.BaseAddress = new Uri(voyage.BaseUrl.TrimEnd('/') + "/");
				client.Timeout = TimeSpan.FromSeconds(30);
			});

			return services;
		}
	}
}
