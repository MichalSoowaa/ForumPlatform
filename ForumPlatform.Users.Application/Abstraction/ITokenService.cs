using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Users.Application.Abstraction
{
	public interface ITokenService
	{
		string GenerateToken(Guid userId, string email, string username);
	}
}
