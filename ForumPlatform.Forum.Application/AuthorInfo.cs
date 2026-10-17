using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application
{
	public record AuthorInfo(string Username, decimal Reputation)
	{
		public static readonly AuthorInfo Unknown = new("Unknown", 0);
	}
}
