using ForumPlatform.Reputation.Infrastructure.Persistence;
using ForumPlatform.Reputation.Domain.Entities;
using ForumPlatform.Reputation.Domain.Scoring;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Reputation.Application.Scoring
{
	/// <summary>
	/// The Hangfire job body. Enqueued (fire-and-forget) by Forum module handlers
	/// after a Thread/Comment is created — see DECISIONS.md "Merit Assessment
	/// Pipeline: asynchronous, triggered once after submission".
	///
	/// Heuristic-only for now: computes the score, records whether it WOULD have
	/// escalated (for later analysis), but never calls an LLM. When LLM escalation
	/// is designed and added back, only the branch inside Handle changes — the
	/// enqueueing call sites in Forum's handlers don't need to change at all.
	/// </summary>
	public class ContentScoringJob
	{
		private readonly ReputationDbContext _db;
		private readonly ContentScoreRecalculator _contentScoreRecalculator;

		public ContentScoringJob(ReputationDbContext db, ContentScoreRecalculator contentScoreRecalculator)
		{
			_db = db;
			_contentScoreRecalculator = contentScoreRecalculator;
		}

		public async Task Handle(Guid contentId, QualityContentType contentType, string content)
		{
			var existing = await _db.QualityAssessments.FirstOrDefaultAsync(
				q => q.ContentId == contentId && q.ContentType == contentType);

			var assessment = existing ?? QualityAssessment.CreatePending(contentId, contentType);

			try
			{
				var subScores = HeuristicScorer.Score(content);
				var wouldEscalate = EscalationEvaluator.ShouldEscalate(subScores);

				assessment.ApplyHeuristicScore(subScores.CombinedScore, wouldEscalate);
			}
			catch
			{
				assessment.MarkFailed();
				throw; // Let Hangfire's own retry policy handle transient failures.
			}
			finally
			{
				if (existing is null)
					_db.QualityAssessments.Add(assessment);

				await _db.SaveChangesAsync();
			}

			if (assessment.Status == QualityAssessmentStatus.HeuristicOnly)
			{
				await _contentScoreRecalculator.Recalculate(contentId, contentType, CancellationToken.None);
			}
		}
	}
}
