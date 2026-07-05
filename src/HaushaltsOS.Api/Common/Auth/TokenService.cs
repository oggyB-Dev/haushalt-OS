using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using HaushaltOS.Api.Common.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HaushaltsOS.Api.Common.Auth;

/// <summary>
/// Erstellt signierte Jwt Access Tokens auf Basis der konfigurierten JwtOptions
/// </summary>
/// <param name="options"></param>
public sealed class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _jwtOptions = options.Value;

    /// <inheritdoc/>
    public string CreateAccessToken(AppUser user)
    {
        // Aussagen über den Benutzer, die im Token stehen
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        // Signatur Header + Payload werden mit dem geheimen Key signiert (HS256)
        SymmetricSecurityKey? key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        SigningCredentials? credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken? token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}