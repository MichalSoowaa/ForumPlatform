	using ForumPlatform.Shared;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using System.Buffers.Binary;
	using System.Security.Cryptography;

	namespace ForumPlatform.Users.Domain.Entities.User
	{
		/// <summary>
		/// User aggregate root. Owns identity (email/username), auth (password hash), and reputation.
		/// Needs to implement interface on its own, because inherits from IdentityUser
		/// </summary>
		public class User : IdentityUser<Guid>, IAggregateRoot<Guid>
		{
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

			public byte[] Version { get; set; } = null!;
			public bool IsDeleted { get; set; }

			public void MarkAsDeleted()
			{
				IsDeleted = true;
			}

			public ulong GetVersion()
			{
				return Version is null ? 0 : BinaryPrimitives.ReadUInt64BigEndian(Version);
			}

			//public Guid Id { get; protected set; } = default!;
			public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
			public DateTime UpdatedAt { get; protected set; }
			public bool IsNew => EqualityComparer<Guid>.Default.Equals(Id, default);

			public void SetCreationDate(DateTime createdAt)
			{
				CreatedAt = createdAt;
			}

			public void SetModifiedDate(DateTime updatedAt)
			{
				UpdatedAt = updatedAt;
			}

			/// <summary>
			/// Factory method for creating a new user.
			/// </summary>
			public static User Create(string email, string username)
			{
				return new User
				{
					Id = Guid.NewGuid(),
					Email = email.ToLower(),
					UserName = username,
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
				MarkAsDeleted();
				UpdatedAt = DateTime.UtcNow;
			}
		}
	}
