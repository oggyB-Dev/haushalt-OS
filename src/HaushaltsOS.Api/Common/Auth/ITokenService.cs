using HaushaltOS.Api.Common.Auth;

namespace HaushaltsOS.Api.Common.Auth;

/// <summary>
/// Erstellt Jwt Access Token für authentifizierte Benutzer.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Erstellt ein signiertes Jwt Access Token für den angegebenen Benutzer.
    /// </summary>
    /// <param name="user">Der Benutzer, für den das Token ausgestellt wird.</param>
    /// <returns>Das signierte Token als String</returns>
    string CreateAccessToken(AppUser user);

    /// <summary>
    /// Erzeugt ein neues Refresh Token, speichert dessen Hash in der Datenbank
    /// und gibt den Klartextwert zurück
    /// </summary>
    /// <returns>Das Refresh Token im Klartext für den Client</returns>
    Task<string> CreateRefreshTokenAsync(AppUser user, CancellationToken cancellationToken);
}