using Microsoft.AspNetCore.Identity;

namespace HaushaltOS.Api.Common.Auth;

/// <summary>
/// Anwendungsnutzer. Erbt alle Identity Felder 
/// und ergänzt eigene Eigenschaften
/// </summary>
public sealed class AppUser : IdentityUser<Guid>
{
    /// <summary>
    /// Anzeigename des Nutzers in der Anwendung
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}