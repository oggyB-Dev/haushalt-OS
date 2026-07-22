using HaushaltsOS.Api.Common.Auth;

using HaushaltsOS.Api.Common.DTOs;

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
    Task<string> CreateAccessTokenAsync(AppUser user, CancellationToken cancellationToken);

    /// <summary>
    /// Erzeugt ein neues Refresh Token, speichert dessen Hash in der Datenbank
    /// und gibt den Klartextwert zurück
    /// </summary>
    /// <returns>Das Refresh Token im Klartext für den Client</returns>
    Task<string> CreateRefreshTokenAsync(AppUser user, CancellationToken cancellationToken);

    /// <summary>
    /// Löst ein Refresh Token ein
    /// Prüft Gültigkeit, widerruft das alte 
    /// und stellt ein neues Token Paar aus (Rotation)
    /// </summary>
    /// <returns>Neues Token Paar oder null wenn das Token ungültig ist</returns>
    Task<AuthResponse?> RotateRefreshTokenAsync(string rawToken, CancellationToken cancellationToken);
}