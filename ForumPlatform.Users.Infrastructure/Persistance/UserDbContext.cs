using Microsoft.EntityFrameworkCore;
using ForumPlatform.Users.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ForumPlatform.Users.Domain.Entities;

namespace ForumPlatform.Users.Infrastructure.Persistance
{
	/// <summary>
	///	ASP.NET Core Identity DbContext for the Users module, using IdentityUser and IdentityRole with Guid as the key type.
	/// </summary>
	public class UserDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public DbSet<User> Users { get; set; } = null!;

		public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
		{}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new UserConfiguration());

			// Put all Users module tables in the "users" schema
			foreach(var entity in modelBuilder.Model.GetEntityTypes())
			{
				if(entity.GetSchema() is null)
					entity.SetSchema("users");
			}

			// Entity configurations land here as they're added, e.g.:
			// modelBuilder.ApplyConfiguration(new UserConfiguration());
			// Each configuration class will pin its table to a schema:
			// builder.ToTable("users", schema: "forum");
		}
	}
}
