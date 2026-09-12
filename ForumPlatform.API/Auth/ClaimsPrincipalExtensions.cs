using Microsoft.Net.Http.Headers;
using System.Security.Claims;

namespace ForumPlatform.API.Auth
{
	public static class ClaimsPrincipalExtensions
	{
		public static Guid GetUserId(this ClaimsPrincipal user)
		{
			var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
				?? throw new InvalidOperationException("No NameIdentifier claim present.");

			return Guid.Parse(idClaim);
		}
	}
}
