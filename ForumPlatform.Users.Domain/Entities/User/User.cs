using ForumPlatform.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ForumPlatform.Users.Domain.Entities.User
{
	/// <summary>
	/// User aggregate root. Owns identity (email/username), auth (password hash), and reputation.
	/// </summary>
	public class User : AggregateRoot<Guid>, IdentityUser<Guid>
	{
		public string Email { get; private set; } = String.Empty;
		public string Username { get; private set; } = String.Empty;
		public string PasswordHash { get; private set; } = String.Empty;
		public decimal TotalReputation { get; private set; }

		/// <summary>
		/// Merit indicator — a rolling average of the quality of the user's recent posts.
		/// Frozen (doesn't decrease) when the user is inactive.
		/// Used alongside TotalReputation to weight the user's votes.
		/// 
		/// Scale: 0–1 (same as individual post scores).
		/// </summary>
		public decimal MeritIndicator { get; private set; }

		public DateTime LastActiveAt { get; private set; }

		/// <summary>
		/// Factory method for creating a new user.
		/// </summary>
		public static User Create(string email, string username, string passwordHash)
		{
			if(string.IsNullOrWhiteSpace(passwordHash))
				throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));

			return new User
			{
				Id = Guid.NewGuid(),
				Email = email.ToLower(),
				Username = username,
				PasswordHash = passwordHash,
				TotalReputation = 0,
				MeritIndicator = 0.5m,
				LastActiveAt = DateTime.UtcNow
			};
		}

		public void AddReputation(decimal amount)
		{
			TotalReputation += amount;
			LastActiveAt = DateTime.UtcNow;
		}

		public void SetMeritIndicator(decimal merit)
		{
			if (merit < 0 || merit > 1)
				throw new ArgumentOutOfRangeException(nameof(merit), "Merit indicator must be between 0 and 1.");

			MeritIndicator = merit;
			UpdateLastActive();
		}

		public void UpdateLastActive()
		{
			LastActiveAt = DateTime.UtcNow;
			UpdatedAt = DateTime.UtcNow;
		}

		public void Delete()
		{
			IsDeleted = true;
			UpdatedAt = DateTime.UtcNow;
		}
	}
}
