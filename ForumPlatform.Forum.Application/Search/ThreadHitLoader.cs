using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Forum.Infrastructure.Search;
using ForumPlatform.Users.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Forum.Application.Search
{
	public class ThreadHitLoader
	{
		private const int SnippetLength = 200;

		private readonly ForumDbContext _forumDb;
		private readonly UserDbContext _userDb;

		public ThreadHitLoader(ForumDbContext forumDb, UserDbContext userDb)
		{
			_forumDb = forumDb;
			_userDb = userDb;
		}

		public async Task<List<ThreadSearchHit>> LoadAsync(IReadOnlyList<ThreadMatch> matches, CancellationToken ct)
		{
			if (matches.Count == 0)
				return new List<ThreadSearchHit>();

			var ids = matches.Select(m => m.ThreadId).ToList();

			var threads = await _forumDb.Threads
				.Where(t => ids.Contains(t.Id))
				.Select(t => new { t.Id, t.Title, t.Body, t.SubforumId, t.AuthorId, t.CommentCount, t.FinalScore, t.CreatedAt })
				.ToListAsync(ct);

			var subforumIds = threads.Select(t => t.SubforumId).Distinct().ToList();
			var subforumNames = await _forumDb.Subforums
				.Where(s => subforumIds.Contains(s.Id))
				.ToDictionaryAsync(s => s.Id, s => s.Name, ct);

			var authorIds = threads.Select(t => t.AuthorId).Distinct().ToList();
			var usernames = await _userDb.Users
				.Where(u => authorIds.Contains(u.Id))
				.ToDictionaryAsync(u => u.Id, u => u.UserName ?? "unknown", ct);

			var byId = threads.ToDictionary(t => t.Id);

			return matches
				.Where(m => byId.ContainsKey(m.ThreadId))
				.Select(m =>
				{
					var t = byId[m.ThreadId];

					return new ThreadSearchHit(
						t.Id, t.Title, Snippet(t.Body), t.SubforumId, subforumNames.GetValueOrDefault(t.SubforumId, "unknown"),
						usernames.GetValueOrDefault(t.AuthorId, "unknown"),
						t.CommentCount, t.FinalScore, t.CreatedAt, m.Similarity);
				})
				.ToList();
		}

		private static string Snippet(string body)
			=> body.Length <= SnippetLength ? body : body[..SnippetLength].TrimEnd() + "...";
	}
}
