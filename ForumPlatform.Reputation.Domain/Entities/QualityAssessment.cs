using ForumPlatform.Shared;

namespace ForumPlatform.Reputation.Domain.Entities
{
	public enum QualityAssessmentStatus
	{
		Pending,
		HeuristicOnly,
		EscalatedToLlm,
		Completed,
		Failed
	}

	public enum ContentType
	{
		Thread,
		Comment
	}

	/// <summary>
	/// Not an aggregate root - produced by the scoring pipeline and attached to a Comment or Thread by Id
	/// </summary>
	public class QualityAssessment : Entity<Guid>
	{
		public Guid ContentId { get; private set; }
		public ContentType ContentType { get; private set; }
		public decimal HeuristicScore { get; private set; }
		public decimal? LlmVerdict { get; private set; }
		public string? LlmJustification { get; private set; }
		public decimal FinalScore => LlmVerdict ?? HeuristicScore;
		public QualityAssessmentStatus Status {  get; private set; }
		public DateTime? CompletedAt { get; private set; }

		private QualityAssessment() { }

		public static QualityAssessment CreatePending(Guid contentId, ContentType contentType)
		{
			return new QualityAssessment
			{
				Id = Guid.NewGuid(),
				ContentId = contentId,
				ContentType = contentType,
				Status = QualityAssessmentStatus.Pending,
				CreatedAt = DateTime.UtcNow
			};
		}

		/// <summary>
		/// Record the heuristic score. Caller decides afterward, based on the escalation triggers,
		/// whether to also call the LLM - this method just records the number and marks status accordingly.
		/// </summary>
		public void ApplyHeuristicScore(decimal heuristicScore, bool willEscalate)
		{
			if (heuristicScore < 0 || heuristicScore > 1)
				throw new ArgumentOutOfRangeException
					(nameof(heuristicScore), "Heuristic score must be between 0 and 1.");

			HeuristicScore = heuristicScore;
			Status = willEscalate ? QualityAssessmentStatus.EscalatedToLlm : QualityAssessmentStatus.HeuristicOnly;

			if (!willEscalate)
				CompletedAt = DateTime.UtcNow;
		}

		public void ApplyLlmVerdict(decimal verdict, string justification)
		{
			if (Status != QualityAssessmentStatus.EscalatedToLlm)
				throw new 
					InvalidOperationException($"Cannot apply an LLM verdict to an assessment with status {Status}.");

			if (verdict < 0 || verdict > 1)
				throw new ArgumentOutOfRangeException(nameof(verdict), "LLM verdict must be between 0 and 1.");

			LlmVerdict = verdict;
			LlmJustification = justification;
			Status = QualityAssessmentStatus.Completed;
			CompletedAt = DateTime.UtcNow;
		}

		public void MarkFailed()
		{
			Status = QualityAssessmentStatus.Failed;
			CompletedAt = DateTime.UtcNow;
		}
	}
}
