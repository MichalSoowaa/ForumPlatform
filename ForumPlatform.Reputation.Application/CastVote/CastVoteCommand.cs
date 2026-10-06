using ForumPlatform.Reputation.Domain.Entities;
using MediatR;

namespace ForumPlatform.Reputation.Application.CastVote
{
	public record CastVoteCommand(Guid VoterId, Guid ContentId, QualityContentType ContentType, VoteValue Value) : IRequest<CastVoteResponse>;

	public record CastVoteResponse(Guid VoteId, VoteValue Value, decimal WeightApplied);
}
