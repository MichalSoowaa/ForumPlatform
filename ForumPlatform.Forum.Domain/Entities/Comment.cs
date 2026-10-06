using ForumPlatform.Shared;

namespace ForumPlatform.Forum.Domain.Entities
{
	/// <summary>
	/// Comment aggregate root.
	/// Max nesting depth of 2 levels.
	/// The depth invariant is enforced by the Caller passing in the parent's depth.
	/// </summary>
	public class Comment : AggregateRoot<Guid>
	{
		public string Body { get; private set; } = null!;
		public Guid ThreadId { get; private set; }
		public Guid AuthorId { get; private set; }
		public Guid? ParentCommentId { get; private set; }
		public int Depth { get; private set; }
		public decimal FinalScore { get; private set; } = 0.5m; // Neutral prior until assessed/voted on.

		protected Comment() { }

		/// <summary>
		/// Create a top-level comment (direct reply to the thread, not to another comment).
		/// </summary>
		public static Comment CreateTopLevel(string body, Guid threadId, Guid authorId)
		{
			ValidateBody(body);

			return new Comment
			{
				Id = Guid.NewGuid(),
				Body = body,
				ThreadId = threadId,
				AuthorId = authorId,
				ParentCommentId = null,
				Depth = 0
			};
		}

		/// <summary>
		/// Create reply to an existing comment.
		/// Caller must pass the parent's Depth (loaded from a single-row query - not the whole thread's comment three)
		/// so this method can enforce the max-2-levels invariant without hydrating unrelated data.
		/// </summary>
		public static Comment CreateReply(string body, Guid threadId, Guid authorId, Guid parentCommentId, int parentDepth)
		{
			ValidateBody(body);
			EnsureCanReplyTo(parentDepth);

			return new Comment
			{
				Id = Guid.NewGuid(),
				Body = body,
				ThreadId = threadId,
				AuthorId = authorId,
				ParentCommentId = parentCommentId,
				Depth = parentDepth + 1
			};
		}

		/// <summary>
		/// Throws if replying to a comment at this depth would exceed the 2-level max.
		/// Depth 0 (top-level) can be replied to -> creates Depth 1.
		/// </summary>
		private static void EnsureCanReplyTo(int parentDepth)
		{
			const int maxParentDepth = 1;

			if (parentDepth >= maxParentDepth)
				throw new InvalidOperationException("Cannot reply to this comment - maximum nesting depth reached.");
		}

		private static void ValidateBody(string body)
		{
			if (string.IsNullOrWhiteSpace(body))
				throw new ArgumentException("Comment body cannot be empty.", nameof(body));
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
