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
}