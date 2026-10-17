using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Users.Infrastructure.Persistance;
using ForumPlatform.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Reputation.Application.Scoring
{
	/// <summary>
	/// Applies the effects of voting/scoring to an author's standing:
	///  - TotalReputation: cumulative sum of signed vote weights (can go negative).
	///  - QualityIndicator: average FinalScore of the author's last N posts.
	///
	/// The QualityIndicator is only recomputed when one of the author's posts is
	/// scored or voted on, so it stays frozen while the author is inactive. 
	/// </summary>
	public class AuthorService
	{
		private const int QualityWindow = 20;
		private const int MaxAttempts = 3;

		private readonly UserDbContext _userDb;
		private readonly ForumDbContext _forumDb;

		public AuthorService(UserDbContext userDb, ForumDbContext forumDb)
		{
			_userDb = userDb;
			_forumDb = forumDb;
		}

		public Task ApplyReputationDelta(Guid authorId, decimal delta, CancellationToken cancellationToken)
			=> UpdateUser(authorId, user => user.AddReputation(delta), cancellationToken);

		/// <summary>
		/// User has an xmin concurrency token, so two simultaneous votes on the
		/// same author's content can collide. Both operations here are safe to
		/// re-apply to a freshly loaded row (a delta, and a value recomputed from
		/// data), so on conflict we reload and try again.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="apply"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task UpdateUser(Guid userId, Action<User> apply, CancellationToken cancellationToken)
		{
			for (var attempt = 1; ; attempt++)
			{
				var user = await _userDb.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
				if (user is null) return; // Deleted user, nothing to do.

				apply(user);

				try
				{
					await _userDb.SaveChangesAsync(cancellationToken);
					return;
				}
				catch (DbUpdateConcurrencyException) when (attempt < MaxAttempts)
				{
					_userDb.ChangeTracker.Clear(); // Discard the stale entity and try again.
				}
			}
		}

		public async Task RefreshQualityIndicator(Guid authorId, CancellationToken cancellationToken)
		{
			var threadScores = await _forumDb.Threads
				.Where(t => t.AuthorId == authorId)
				.OrderByDescending(t => t.CreatedAt)
				.Take(QualityWindow)
				.Select(t => new { t.CreatedAt, t.FinalScore })
				.ToListAsync(cancellationToken);

			var commentScores = await _forumDb.Comments
				.Where(c => c.AuthorId == authorId)
				.OrderByDescending(c => c.CreatedAt)
				.Take(QualityWindow)
				.Select(c => new { c.CreatedAt, c.FinalScore })
				.ToListAsync(cancellationToken);

			var recent = threadScores.Concat(commentScores)
				.OrderByDescending(x => x.CreatedAt)
				.Take(QualityWindow)
				.ToList();

			if (recent.Count == 0)
				return;

			var average = Math.Round(recent.Average(x => x.FinalScore), 4);

			await UpdateUser(authorId, user => user.SetQualityIndicator(average), cancellationToken);
		}
	}
}
