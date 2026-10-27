using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Search
{
	public record ThreadSearchHit(
		Guid Id,
		string Title,
		string Snippet,
		Guid SubforumId,
		string SubforumName,
		string AuthorUsername,
		int CommentCount,
		decimal FinalScore,
		DateTime CreatedAt,
		double? Similarity // null = matched by keywords only
	);
}
