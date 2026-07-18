using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.DTOs;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HaushaltsOS.Api.Features.Auth.Register;

/// <summary>
/// Endpoint für die Registrierung eines neuen Benutzers
/// </summary>
public static class RegisterEndpoint
{
    /// <summary>
    /// Registriert die Route für die Benutzerregistrierung
    /// </summary>
    public static void MapRegister(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", HandleAsync);
    }

    private static async Task<IResult> HandleAsync([FromBody] RegisterRequest request, UserManager<AppUser> userManager, ITokenService tokenService, CancellationToken cancellationToken)
    {
        // Prüfen, ob die Email bereits vergeben ist
        AppUser? existing = await userManager.FindByEmailAsync(request.Email!);

        if(existing is not null)
        {
            return Results.Problem(
                title: "Email bereits vergeben",
                statusCode: StatusCodes.Status409Conflict
            );
        }

        AppUser user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName!
        };
        
        // Passwort hashen und User speichern
        IdentityResult result = await userManager.CreateAsync(user, request.Password!);

        if(!result.Succeeded)
        {
            return Results.Problem(
                title: "Registrierung fehlgeschlagen",
                detail: string.Join("; ", result.Errors.Select(err => err.Description)),
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        string accessToken = tokenService.CreateAccessToken(user);
        string refreshToken = await tokenService.CreateRefreshTokenAsync(user, cancellationToken);

        return Results.Ok(new AuthResponse(
            accessToken,
            refreshToken
        ));

    }
}