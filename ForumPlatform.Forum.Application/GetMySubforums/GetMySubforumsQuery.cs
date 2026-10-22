using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.GetMySubforums
{
	public record GetMySubforumsQuery(Guid UserId) : IRequest<List<MySubforum>>;

	public record MySubforum(
		Guid Id, string Name, string Description, string Status,
		List<string> Tags, string? RejectionReason, DateTime CreatedAt);
}
