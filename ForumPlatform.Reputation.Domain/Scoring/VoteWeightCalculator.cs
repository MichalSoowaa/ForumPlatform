using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Reputation.Domain.Scoring
{
	/// <summary>
	/// Vote weight scales logarithmically with voter reputation,
	/// hard-capped - this prevents high-reputation users from disproportonately dominating scores via unbounded linear scaling.
	/// </summary>
	public static class VoteWeightCalculator
	{
		private const decimal ScalingFactor = 0.5m; // Adjust this factor to control the scaling rate
		private const decimal MaxMultiplier = 5.0m;

		/// <summary>
		/// Calculates the weight of a vote based on the voter's reputation.
		/// Log is only defined for positive input, so negative/zero reputation is treated
		/// as contributing nothing beyond the base weight of 1.0, rather than
		/// throwing or producing a nonsensical negative weight.
		/// </summary>
		/// <param name="reputation"></param>
		/// <returns></returns>
		public static decimal CalculateWeight(decimal reputation)
		{
			if (reputation <= 0)
				return 1.0m; // Minimum weight for users with no reputation

			var logComponent = (decimal)Math.Log((double)(1 + reputation));
			var weight = 1 + (logComponent * ScalingFactor);

			return Math.Min(weight, MaxMultiplier); // Cap the weight to prevent excessive influence
		}
	}
}
