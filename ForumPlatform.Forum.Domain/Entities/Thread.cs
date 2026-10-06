using ForumPlatform.Shared;

namespace ForumPlatform.Forum.Domain.Entities
{
	/// <summary>
	/// Thread aggregate root - the opening post only.
	/// Deliberately does NOT include comments as child entities
	/// (avoids loading huge object graphs just to check an invariant)
	/// 
	/// FinalScore/embedding are NOT here yet - those belong to the Reputation and Search capabilities respectively.
	/// Adding them later is a non-breaking column addition, not a redesign, so there's no reason to stub them out now.
	/// </summary>
	public class Thread : AggregateRoot<Guid>
	{
		public string Title { get; private set; } = null!;
		public string Body { get; private set;  } = null!;
		public Guid SubforumId { get; private set; }
		public Guid AuthorId { get; private set; }
		public int CommentCount { get; private set; }
		public DateTime LastActivityAt { get; private set; }
		public decimal FinalScore { get; private set; } = 0.5m; // Neutral prior until assessed/voted on.

		protected Thread() { }

		public static Thread Create(string title, string body, Guid subforumId, Guid authorId)
		{
			if (string.IsNullOrWhiteSpace(title))
				throw new ArgumentException("Thread title cannot be empty.", nameof(title));

			if (string.IsNullOrWhiteSpace(body))
				throw new ArgumentException("Thread body cannot be empty.", nameof(body));

			return new Thread
			{
				Id = Guid.NewGuid(),
				Title = title,
				Body = body,
				SubforumId = subforumId,
				AuthorId = authorId,
				CommentCount = 0,
				LastActivityAt = DateTime.UtcNow,
				FinalScore = 0.5m,
			};
		}

		public void RegisterNewComment()
		{
			CommentCount++;
			LastActivityAt = DateTime.UtcNow;
			UpdatedAt = DateTime.UtcNow;
		}

		/// <summary>
		/// Overwrites the cached, displayed quality score. Called by the Reputation module's ContentScoreRecalculator.
		/// </summary>
		/// <param name="finalScore"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public void ApplyFinalScore(decimal finalScore)
		{
			if (finalScore < 0 || finalScore > 1)
				throw new ArgumentOutOfRangeException(nameof(finalScore), "Final score must be between 0 and 1.");

			FinalScore = finalScore;
			UpdatedAt = DateTime.UtcNow;
		}
	}
}
