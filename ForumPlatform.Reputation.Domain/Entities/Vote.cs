using ForumPlatform.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Reputation.Domain.Entities
{
	public enum VoteValue
	{
		Upvote = 1,
		Downvote = -1
	}

	/// <summary>
	/// One vote per content.
	/// WeightAtCast is snapshotted, not recalculated retroactively if the voter's own reputation changes later.
	/// </summary>
	public class Vote : AggregateRoot<Guid>
	{
		public Guid VoterId { get; private set; }
		public Guid ContentId { get; private set; }
		public ContentType ContentType { get; private set; }
		public VoteValue Value { get; private set; }
		public decimal WeightAtCast { get; private set; }

		/// <summary>
		/// This vote's contribution to its content author's TotalReputation,
		/// taking into account the vote's value (upvote or downvote) and the voter's weight at the time of casting.
		/// </summary>
		public decimal SignedWeight => (int)Value * WeightAtCast;

		protected Vote() { } // For EF Core

		public static Vote Cast(Guid voterId, Guid contentId, ContentType contentType, VoteValue value, decimal weightAtCast)
		{
			if (weightAtCast <= 0)
				throw new ArgumentException("Weight at cast must be positive.", nameof(weightAtCast));

			return new Vote
			{
				Id = Guid.NewGuid(),
				VoterId = voterId,
				ContentId = contentId,
				ContentType = contentType,
				Value = value,
				WeightAtCast = weightAtCast
			};
		}

		public void ChangeVote(VoteValue newValue, decimal newWeight)
		{
			if (newValue == Value)
				throw new InvalidOperationException("Cannot change to the same vote value.");

			if (newWeight <= 0)
				throw new ArgumentException("Weight at cast must be positive.", nameof(newWeight));

			Value = newValue;
			WeightAtCast = newWeight;
			UpdatedAt = DateTime.UtcNow;
		}
	}
}
