using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Reputation.Domain.Scoring
{
	/// <summary>
	/// NOT currently wired into the scoring pipeline — LLM escalation is deferred
	/// pending a redesign of the scoring approach. ShouldEscalate is still called
	/// by the pipeline for now, purely to record QualityAssessment.WouldHaveEscalated
	/// for later analysis; the result does not currently trigger any LLM call.
	/// </summary>
	public static class EscalationEvaluator
	{
		private const decimal MidBandLower = 0.35m;
		private const decimal MidBandUpper = 0.65m;

		// A sub-score more than this far from the combined score counts as conflicting
		private const decimal ConflictTreshold = 0.4m;

		public static bool ShouldEscalate(HeuristicSubScores subScores)
		{
			return IsMidConfidenceBand(subScores.CombinedScore)
				|| HasSubScoreConflict(subScores)
				|| subScores.ContainsCodeOrMath;
		}

		private static bool IsMidConfidenceBand(decimal combinedScore)
		{
			return combinedScore >= MidBandLower && combinedScore <= MidBandUpper;
		}

		/// <summary>
		/// Caches cases like "long but unstructured" or "has a link but almost no surrounding text"
		/// - any individual sub-score far from the combined total suggests the heuristic's overall number is masking
		/// a mismatch between signals, rather than genuine agreement between them.
		/// </summary>
		private static bool HasSubScoreConflict(HeuristicSubScores subScores)
		{
			var combined = subScores.CombinedScore;

			return Math.Abs(subScores.LengthScore - combined) > ConflictTreshold
				|| Math.Abs(subScores.SourceScore - combined) > ConflictTreshold
				|| Math.Abs(subScores.StructureScore - combined) > ConflictTreshold;
		}
	}
}
