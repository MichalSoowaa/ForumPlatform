using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Moderation.ApproveSubforum
{
	public record ApproveSubforumCommand(Guid SubforumId, Guid ModeratorId, List<string> Tags) : IRequest<SubforumReviewResult>;
}
