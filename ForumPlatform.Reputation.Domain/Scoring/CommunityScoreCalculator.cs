using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Reputation.Domain.Scoring
{
	/// <summary>
	/// Bayesian-average community score calculator.
	/// Avoids unreliable scores for content with few votes by pulling the result toward a global average (priorScore) when vote count is low,
	/// and toward the actual weighted vote ratio as votes accumulate.
	/// 
	/// Operates on weighted sums (vote weight from VoteWeightCalculator),
	/// not raw vote counts - a single high-reputation upvote counts for more than a single low-reputation one.
	/// </summary>
	public static class CommunityScoreCalculator
	{
		/// <summary>
		/// C in the Bayesian-average formula: how many "virtual" votes-worth of weight the prior is worth.
		/// Higher = more resistent to being swayed by a handful of early votes; lower = converges to the raw ratio faster.
		/// </summary>
		private const decimal PriorWeight = 10m;

		public static decimal Calculate(decimal weightedUpvotes, decimal weightedDownvotes, decimal priorScore = 0.5m)
		{
			decimal totalWeight = weightedUpvotes + weightedDownvotes;

			// No votes at all: return the prior outright - avoids a 0/0 division, and matches the intent (no signal yet, so return the prior).
			if (totalWeight <= 0)
			{
				return priorScore;
			}

			return (weightedUpvotes + (PriorWeight * priorScore)) / (totalWeight + PriorWeight);
		}
	}
}
