using ForumPlatform.Forum.Application.Moderation.ApproveSubforum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Moderation.RejectSubforum
{
	public record RejectSubforumCommand(Guid SubforumId, Guid ModeratorId, string Reason)
		: IRequest<SubforumReviewResult>;
}
