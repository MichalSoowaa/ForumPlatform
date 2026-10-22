using ForumPlatform.Forum.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Moderation.ApproveSubforum
{
	public class ApproveSubforumCommandHandler : IRequestHandler<ApproveSubforumCommand, SubforumReviewResult>
	{
		private readonly ForumDbContext _forumDb;

		public ApproveSubforumCommandHandler(ForumDbContext forumDb)
		{
			_forumDb = forumDb;
		}

		public async Task<SubforumReviewResult> Handle(ApproveSubforumCommand request, CancellationToken cancellationToken)
		{
			var subforum = await _forumDb.Subforums.FirstOrDefaultAsync(s => s.Id == request.SubforumId, cancellationToken)
				?? throw new InvalidOperationException($"Subforum {request.SubforumId} does not exist.");

			subforum.Approve(request.ModeratorId, request.Tags); // throws if no longer pending

			try
			{
				await _forumDb.SaveChangesAsync(cancellationToken);
			}
			catch (DbUpdateConcurrencyException)
			{
				throw new InvalidOperationException("This subforum was just reviewed by another moderator.");
			}

			return new SubforumReviewResult(subforum.Id, subforum.Status.ToString(), subforum.Tags);
		}
	}
}
