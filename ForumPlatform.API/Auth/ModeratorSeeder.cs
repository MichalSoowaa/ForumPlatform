using ForumPlatform.Shared;
using ForumPlatform.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ForumPlatform.API.Auth
{
	public static class ModeratorSeeder
	{
		public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
		{
			var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
			var userManager = services.GetRequiredService<UserManager<User>>();

			if (!await roleManager.RoleExistsAsync(Roles.Moderator))
				await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Moderator));

			var emails = configuration.GetSection("Moderation:InitialModeratorEmails").Get<string[]>()
				?? Array.Empty<string>();

			foreach (var email in emails)
			{
				var user = await userManager.FindByEmailAsync(email);
				if (user is null)
				{
					logger.LogWarning("Moderator with email {Email} not found. Skipping.", email);
					continue;
				}

				if (!await userManager.IsInRoleAsync(user, Roles.Moderator))
					await userManager.AddToRoleAsync(user, Roles.Moderator);
			}
		}
	}
}
