using System.ComponentModel.DataAnnotations;

namespace HaushaltsOS.Api.Features.Auth.Register;

/// <summary>
/// Anfrage zur Registrierung eines neuen Benutzers
/// </summary>
public sealed class RegisterRequest
{
    /// <summary>
    /// Email Adresse des neuen Benutzers
    /// </summary>
    [Required, EmailAddress]
    public string? Email { get; init; }

    /// <summary>
    /// Anzeigename des neuen Benutzers
    /// </summary>
    [Required, MinLength(2)]
    public string? DisplayName { get; init; }

    /// <summary>
    /// Passwort des neuen Benutzers
    /// </summary>
    [Required, MinLength(8)]
    public string? Password { get; init; }
}