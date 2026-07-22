using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HaushaltsOS.Api.Common.Auth;

/// <summary>
/// Liest Identität und Haushalt des angemeldeten Benutzers aus den Token Claims
/// </summary>
/// <param name="httpContextAccessor"></param>
public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor)
{
    private ClaimsPrincipal Principal => 
        httpContextAccessor.HttpContext?.User 
        ?? throw new InvalidOperationException("Kein HTTP Kontext vorhanden");
    
    /// <summary>
    /// Id des angemeldeten Benutzers
    /// </summary>
    public Guid UserId => 
        Guid.Parse(Principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? throw new InvalidOperationException("Kein Benutzer angemeldet."));
    
    /// <summary>
    /// Haushalt des angemeldeten Benutzers
    /// </summary>
    public Guid HouseholdId => 
        Guid.Parse(Principal.FindFirstValue("household_id")
        ?? throw new InvalidOperationException("Benutzer gehört keinem Haushalt an."));
}