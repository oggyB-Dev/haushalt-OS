namespace HaushaltsOS.Api.Common.Auth;

/// <summary>
/// Konfigurationswerte für die Jwt Erstellung
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Der geheime Signaturschlüssel
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Der Aussteller des Tokens
    /// </summary>
    public required string Issuer { get; init; }

    /// <summary>
    /// Die vorgesehene Zielgruppe des Tokens
    /// </summary>
    public required string Audience { get; init; }

    /// <summary>
    /// Gültigkeitsdauer des Access Tokens in Minuten
    /// </summary>
    public int AccessTokenMinutes { get; init; } = 15;
}