using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Reputation.Domain.Entities;
using ForumPlatform.Reputation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ForumPlatform.Reputation.Domain.Scoring;

namespace ForumPlatform.Reputation.Application.Scoring
{
	/// <summary>
	/// Recomputes and persists a piece of content's displayed FinalScore
	/// - combining its QualityAssessment with its vote tally via CommunityScoreCalculator + FinalScoreCalculator.
	/// 
	/// Called from two places: CastVoteCommandHandler (vote tally changed) and ContentScoringJob (quality assessment just completed)
	/// - both situations where the inputs to FinalScore have changed, so the cached value needs recomputing.
	/// TODO(reputation-module-extraction)
	/// </summary>
	public class ContentScoreRecalculator
	{
		private readonly ReputationDbContext _reputationDb;
		private readonly ForumDbContext _forumDb;

		public ContentScoreRecalculator(ReputationDbContext reputationDb, ForumDbContext forumDb)
		{
			_reputationDb = reputationDb;
			_forumDb = forumDb;
		}

		public async Task Recalculate(Guid contentId, QualityContentType contentType, CancellationToken cancellationToken)
		{
			var votes = await _reputationDb.Votes
				.Where(v => v.ContentId == contentId && v.ContentType == contentType)
				.Select(v => new { v.Value, v.WeightAtCast })
				.ToListAsync(cancellationToken);

			var weightedUpvotes = votes.Where(v => v.Value == VoteValue.Upvote).Sum(v => v.WeightAtCast);
			var weightedDownvotes = votes.Where(v => v.Value == VoteValue.Downvote).Sum(v => v.WeightAtCast);

			var communityScore = CommunityScoreCalculator.Calculate(weightedUpvotes, weightedDownvotes);

			// Heuristic-only for now — FinalScore on QualityAssessment already
			// falls back to HeuristicScore when no LLM verdict exists (see its
			// definition), so no branching needed here even once LLM escalation
			// is wired in later.
			var qualityScore = await _reputationDb.QualityAssessments.Where(q => q.ContentId == contentId && q.ContentType == contentType)
				.Select(q => (decimal?)q.FinalScore)
				.FirstOrDefaultAsync(cancellationToken) ?? 0.5m;

			var finalScore = FinalScoreCalculator.Calculate(qualityScore, communityScore, votes.Count);

			switch (contentType)
			{
				case QualityContentType.Thread:
					var thread = await _forumDb.Threads.FirstOrDefaultAsync(t => t.Id == contentId, cancellationToken);
					thread?.ApplyFinalScore(finalScore);
					break;
				case QualityContentType.Comment:
					var comment = await _forumDb.Comments.FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);
					comment?.ApplyFinalScore(finalScore);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(contentType), $"Unsupported content type: {contentType}");
			}

			await _forumDb.SaveChangesAsync(cancellationToken);
		}
	}
}
