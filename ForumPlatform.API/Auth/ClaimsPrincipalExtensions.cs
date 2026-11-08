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

		// For endpoints that work for anonymous visitors but add personal data when logged in.
		public static Guid? TryGetUserId(this ClaimsPrincipal user)
		{
			var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return Guid.TryParse(claim, out var id) ? id : null;
		}
	}
}
