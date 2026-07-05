using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using HaushaltOS.Api.Common.Auth;
using HaushaltOS.Api.Common.Persistence;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HaushaltsOS.Api.Common.Auth;

/// <summary>
/// Erstellt signierte Jwt Access Tokens auf Basis der konfigurierten JwtOptions
/// </summary>
/// <param name="options"></param>
public sealed class TokenService(IOptions<JwtOptions> options, AppDbContext dbContext) : ITokenService
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

    /// <inheritdoc/>
    public async Task<string> CreateRefreshTokenAsync(AppUser user, CancellationToken cancellationToken)
    {
        // Zufallswert erzeugen 256 Bit
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
        string rawToken = Convert.ToBase64String(randomBytes);

        // Hash speichern
        string tokenHash = HashToken(rawToken);

        RefreshToken refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Tokenhash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
        };

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Klartext an den Aufrufer zurück 
        return rawToken;
    }

    /// <summary>
    /// Berechnet den SHA 256 Hash eines Tokens als Hex String
    /// </summary>
    private static string HashToken(string token)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}