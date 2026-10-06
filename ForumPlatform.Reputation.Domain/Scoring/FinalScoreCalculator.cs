using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Reputation.Domain.Scoring
{
	/// <summary>
	/// Combines the quality-assessment score with the community score.
	/// Alpha decreases as vote count grows: trust the automated assessment early when there's little community signal,
	/// trust the community more as votes accumulate.
	/// </summary>
	public static class FinalScoreCalculator
	{
		private const decimal AlphaScalingConstant = 10m;

		public static decimal Calculate(decimal qualityScore, decimal communityScore, int totalVoteCount)
		{
			decimal alpha = AlphaScalingConstant / (AlphaScalingConstant + totalVoteCount);
			return (alpha * qualityScore) + ((1 - alpha) * communityScore);
		}
	}
}
