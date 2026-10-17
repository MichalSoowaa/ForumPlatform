using ForumPlatform.Forum.Infrastructure.Persistance;
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
		private readonly ForumDbContext _forumDb;
		private readonly ContentScoreRecalculator _contentScoreRecalculator;
		private readonly AuthorService _authorService;

		public CastVoteCommandHandler(
			ReputationDbContext reputationDb,
			UserDbContext userDb,
			ContentScoreRecalculator contentScoreRecalculator,
			ForumDbContext forumDb,
			AuthorService authorService)
		{
			_reputationDb = reputationDb;
			_userDb = userDb;
			_contentScoreRecalculator = contentScoreRecalculator;
			_forumDb = forumDb;
			_authorService = authorService;
		}

		public async Task<CastVoteResponse> Handle(CastVoteCommand request, CancellationToken cancellationToken)
		{
			// Direct cross-module query for a single column — deliberately kept
			// to the lightest possible read (no User entity materialized), same
			// spirit as CreateCommentCommandHandler's parent-depth lookup.
			//
			// TODO(reputation-module-extraction): this direct reference to
			// Users.Infrastructure is a known, deliberate shortcut

			var contentAuthorId = await GetContentAuthorId(request.ContentId, request.ContentType, cancellationToken);

			if (contentAuthorId is null)
				throw new InvalidOperationException($"{request.ContentType} {request.ContentId} does not exist.");

			if (contentAuthorId == request.VoterId)
				throw new InvalidOperationException("Users cannot vote on their own content.");

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

			// Capture BEFORE ChangeVote mutates the row: this is the amount the
			// author's reputation currently includes from this voter.
			var previousContribution = existingVote?.SignedWeight ?? 0m;

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

			var delta = vote.SignedWeight - previousContribution;

			if (delta != 0m)
			{
				await _authorService.ApplyReputationDelta(contentAuthorId.Value, delta, cancellationToken);
			}

			// Recompute and persist the content's displayed FinalScore now that the vote tally has changed.
			await _contentScoreRecalculator.Recalculate(request.ContentId, request.ContentType, cancellationToken);

			return new CastVoteResponse(vote.Id, vote.Value, vote.WeightAtCast);
		}

		/// <summary>
		/// Single-column lookup: returns null when the content doesn't exist
		/// (or is soft-deleted), otherwise its author's Id. One query answers
		/// both "does it exist" and "is the voter its author". 
		/// </summary>
		/// <param name="contentId"></param>
		/// <param name="contentType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private Task<Guid?> GetContentAuthorId(Guid contentId, ContentType contentType, CancellationToken cancellationToken)
		{
			return contentType switch
			{
				ContentType.Comment => _forumDb.Comments
					.Where(c => c.Id == contentId)
					.Select(c => (Guid?)c.AuthorId)
					.FirstOrDefaultAsync(cancellationToken),

				ContentType.Thread => _forumDb.Threads
					.Where(p => p.Id == contentId)
					.Select(p => (Guid?)p.AuthorId)
					.FirstOrDefaultAsync(cancellationToken),

				_ => Task.FromResult<Guid?>(null),
			};
		}
	}
}
