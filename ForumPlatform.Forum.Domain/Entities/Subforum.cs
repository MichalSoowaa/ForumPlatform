using ForumPlatform.Shared;

namespace ForumPlatform.Forum.Domain.Entities
{
	/// <summary>
	/// Subforum aggregate root. Created by a user, requires moderator approval.
	/// </summary>
	public class Subforum : AggregateRoot<Guid>
	{
		public string Name { get; private set; } = null!;
		public string Description { get; private set; } = null!;
		public SubforumStatus Status { get; private set; }

		/// <summary>
		/// The user who proposed this subforum. Stored as a plain Guid,
		/// not a navigation property - Forum module doesn't reference Users.Domain directly.
		/// </summary>
		public Guid CreatedByUserId { get; private set; }

		protected Subforum() { }

		public static Subforum Create(string name, string description, Guid createdByUserId)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Subforum name cannot be empty.", nameof(name));

			return new Subforum
			{
				Id = Guid.NewGuid(),
				Name = name,
				Description = description,
				Status = SubforumStatus.Pending,
				CreatedByUserId = createdByUserId
			};
		}

		public void Approve()
		{
			if (Status != SubforumStatus.Pending)
				throw new InvalidOperationException(
					$"Cannot approve a subforum with status {Status}." +
					$" Only pending subforums can be approved.");

			Status = SubforumStatus.Approved;
			UpdatedAt = DateTime.UtcNow;
		}

		public void Reject()
		{
			if (Status != SubforumStatus.Pending)
				throw new InvalidOperationException(
					$"Cannot reject a subforum with status {Status}." +
					$"Only pending subforums can be rejected.");

			Status = SubforumStatus.Rejected;
			UpdatedAt = DateTime.UtcNow;
		}
	}
}
