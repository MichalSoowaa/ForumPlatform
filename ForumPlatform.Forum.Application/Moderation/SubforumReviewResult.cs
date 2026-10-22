using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Moderation
{
	public record SubforumReviewResult(Guid SubforumId, string Status, List<string> Tags);
}
