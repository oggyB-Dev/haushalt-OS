namespace HaushaltsOS.Api.Common.Auth;

/// <summary>
/// Ein in der Datenbank gespeichertes Refresh Token 
/// Dient dazu, neue Access Token auszustellen, und ist widerrufbar
/// </summary>
public sealed class RefreshToken
{
    /// <summary>
    /// Eindeutige Id des Refresh Tokens
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Der Benutzer, zu dem dieses Token gehört 
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// SHA 256 Hash des Tokens 
    /// </summary>
    public required string Tokenhash { get; init; }

    /// <summary>
    /// Zeitpunkt, zu dem das Token abläuft
    /// </summary>
    public DateTime ExpiresAtUtc { get; init; }
    
    /// <summary>
    /// Zeitpunkt der Erstellung
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Zeitpunkt des Widerrufs, falls widerrufen
    /// </summary>
    public DateTime? RevokedAtUtc { get; set; }

    /// <summary>
    /// Gibt an, ob das Token aktuell gültig ist
    /// </summary>
    public bool IsActive => RevokedAtUtc is null && DateTime.UtcNow < ExpiresAtUtc;
}