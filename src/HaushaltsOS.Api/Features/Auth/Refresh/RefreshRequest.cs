using System.ComponentModel.DataAnnotations;

namespace HaushaltsOS.Api.Features.Auth.Refresh;

/// <summary>
/// Anfrage zur Erneuerung der Token
/// </summary>
public sealed class RefreshRequest
{
    /// <summary>
    /// Das Refresh Token des Nutzers zur Verlängerung der Sitzung
    /// </summary>
    [Required]
    public string? RefreshToken { get; set; }
}