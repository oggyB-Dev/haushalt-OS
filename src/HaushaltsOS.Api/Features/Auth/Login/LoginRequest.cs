using System.ComponentModel.DataAnnotations;

namespace HaushaltsOS.Api.Features.Auth.Login;

/// <summary>
/// Anfrage zur Anmeldung eines Nutzers
/// </summary>
public sealed class LoginRequest
{   
    /// <summary>
    /// Die Email Adresse des bereits registrierten Nutzers
    /// </summary>
    [Required, EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Das Passwort des bereits registrierten Nutzers.
    /// </summary>
    [Required]
    public string? Password { get; set; }
}
