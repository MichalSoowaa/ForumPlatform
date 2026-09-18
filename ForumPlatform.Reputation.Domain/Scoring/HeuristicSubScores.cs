using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Reputation.Domain.Scoring
{
	/// <summary>
	/// The individual sub-scores that make up the heuristic score, kept visible
	/// as their own record rather than collapsed immediately into one number.
	/// These are passed into the LLM prompt as context when escalation happens
	/// </summary>
	public record HeuristicSubScores(
		decimal LengthScore,
		decimal SourceScore,
		decimal StructureScore,
		decimal SpamPenalty,
		bool ContainsCodeOrMath
	)
	{
		/// <summary>
		/// Weighted sum of the positive sub-scores, reduced by the spam penalty as a multiplier
		/// </summary>
		public decimal CombinedScore
		{
			get
			{
				const decimal lengthWeight = 0.4m;
				const decimal sourceWeight = 0.2m;
				const decimal structureWeight = 0.4m;

				var weighted = (LengthScore * lengthWeight)
					+ (SourceScore * sourceWeight)
					+ (StructureScore * structureWeight);

				return weighted * (1 - SpamPenalty);
			}
		}
	}
}
