using ForumPlatform.Users.Application.Abstraction;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ForumPlatform.API.Auth
{
	public class TokenService : ITokenService
	{
		private readonly string _signingKey;
		private readonly string _issuer;
		private readonly string _audience;
		private readonly int _expirationMinutes;

		public TokenService(IConfiguration configuration)
		{
			_signingKey = configuration["Jwt:SigningKey"]
				?? throw new InvalidOperationException("JWT signing key is not configured.");

			_issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");

			_audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");

			_expirationMinutes = int.TryParse(configuration["Jwt:ExpirationMinutes"], out var expiration)
				? expiration : 1000;
		}

		public string GenerateToken(Guid userId, string email, string username)
		{
			var signingKeyBytes = Encoding.UTF8.GetBytes(_signingKey);

			var signingCredentials = new SigningCredentials(
				new SymmetricSecurityKey(signingKeyBytes),
				SecurityAlgorithms.HmacSha256Signature);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
				new Claim(ClaimTypes.Email, email),
				new Claim(ClaimTypes.Name, username)
			};

			return "generated_token";
		}
	}
}
