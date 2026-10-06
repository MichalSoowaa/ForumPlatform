using ForumPlatform.Reputation.Application.Scoring;
using ForumPlatform.Reputation.Domain.Entities;
using ForumPlatform.Reputation.Domain.Scoring;
using ForumPlatform.Reputation.Infrastructure.Persistence;
using ForumPlatform.Users.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Reputation.Application.CastVote
{
	public class CastVoteCommandHandler : IRequestHandler<CastVoteCommand, CastVoteResponse>
	{
		private readonly ReputationDbContext _reputationDb;
		private readonly UserDbContext _userDb;
		private readonly ContentScoreRecalculator _contentScoreRecalculator;

		public CastVoteCommandHandler(ReputationDbContext reputationDb, UserDbContext userDb, ContentScoreRecalculator contentScoreRecalculator)
		{
			_reputationDb = reputationDb;
			_userDb = userDb;
			_contentScoreRecalculator = contentScoreRecalculator;
		}

		public async Task<CastVoteResponse> Handle(CastVoteCommand request, CancellationToken cancellationToken)
		{
			// Direct cross-module query for a single column — deliberately kept
			// to the lightest possible read (no User entity materialized), same
			// spirit as CreateCommentCommandHandler's parent-depth lookup.
			//
			// TODO(reputation-module-extraction): this direct reference to
			// Users.Infrastructure is a known, deliberate shortcut

			var voterReputation = await _userDb.Users
				.Where(u => u.Id == request.VoterId)
				.Select(u => u.TotalReputation)
				.FirstOrDefaultAsync(cancellationToken);

			var weight = VoteWeightCalculator.CalculateWeight(voterReputation);

			var existingVote = await _reputationDb.Votes
				.FirstOrDefaultAsync(v => v.VoterId == request.VoterId &&
										  v.ContentId == request.ContentId &&
										  v.ContentType == request.ContentType,
									  cancellationToken);

			Vote vote;

			if (existingVote is null)
			{
				vote = Vote.Cast(request.VoterId, request.ContentId, request.ContentType, request.Value, weight);
				_reputationDb.Votes.Add(vote);
			}
			else
			{
				existingVote.ChangeVote(request.Value, weight);
				vote = existingVote;
			}

			await _reputationDb.SaveChangesAsync(cancellationToken);

			// Recompute and persist the content's displayed FinalScore now that the vote tally has changed.
			await _contentScoreRecalculator.Recalculate(request.ContentId, request.ContentType, cancellationToken);

			return new CastVoteResponse(vote.Id, vote.Value, vote.WeightAtCast);
		}
	}
}
