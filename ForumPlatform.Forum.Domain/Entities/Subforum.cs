using ForumPlatform.Shared;

namespace ForumPlatform.Forum.Domain.Entities
{
	/// <summary>
	/// Subforum aggregate root. Created by a user, requires moderator approval.
	/// </summary>
	public class Subforum : AggregateRoot<Guid>
	{
		public const int MaxTags = 5;
		public const int MaxTagLength = 30;
		public string Name { get; private set; } = null!;
		public string Description { get; private set; } = null!;
		public SubforumStatus Status { get; private set; }

		/// <summary>
		/// The user who proposed this subforum. Stored as a plain Guid,
		/// not a navigation property - Forum module doesn't reference Users.Domain directly.
		/// </summary>
		public Guid CreatedByUserId { get; private set; }

		// Topic tags assigned by the moderator on approval. Stored as a Postgres
		// text[] column. If expert badges (per-tag reputation) are ever added, this
		// is the point to normalize into a Tag table.
		public List<string> Tags { get; private set; } = new();

		public Guid? ReviewedByUserId { get; private set; }
		public DateTime? ReviewedAt { get; private set; }
		public string? RejectionReason { get; private set; }

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

		private void EnsurePending(string action)
		{
			if (Status != SubforumStatus.Pending)
				throw new InvalidOperationException(
					$"Cannot {action} a subforum with status {Status}. Only Pending subforums can be reviewed.");
		}

		private static List<string> NormalizeTags(IEnumerable<string> tags)
		{
			var normalized = tags
				.Select(t => t?.Trim().ToLowerInvariant())
				.OfType<string>()
				.Where(t => t.Length > 0)
				.Distinct()
				.ToList();

			if (normalized.Count == 0)
				throw new ArgumentException("At least one tag is required.", nameof(tags));
			if (normalized.Count > MaxTags)
				throw new ArgumentException($"A subforum can have at most {MaxTags} tags.", nameof(tags));
			if (normalized.Any(t => t.Length > MaxTagLength))
				throw new ArgumentException($"A tag can be at most {MaxTagLength} characters.", nameof(tags));

			return normalized;
		}

		private void MarkReviewed(Guid moderatorId)
		{
			ReviewedByUserId = moderatorId;
			ReviewedAt = DateTime.UtcNow;
			UpdatedAt = DateTime.UtcNow;
		}

		/// <summary>Pending -> Approved, recording who approved it and the tags assigned.</summary>
		public void Approve(Guid moderatorId, IEnumerable<string> tags)
		{
			EnsurePending("approve");
			Tags = NormalizeTags(tags);
			MarkReviewed(moderatorId);
			Status = SubforumStatus.Approved;
		}

		/// <summary>Pending -> Rejected. A reason is mandatory so the proposer can see why.</summary>
		public void Reject(Guid moderatorId, string reason)
		{
			EnsurePending("reject");

			if (string.IsNullOrWhiteSpace(reason))
				throw new ArgumentException("A rejection reason is required.", nameof(reason));

			RejectionReason = reason.Trim();
			MarkReviewed(moderatorId);
			Status = SubforumStatus.Rejected;
		}
	}
}
