using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.DTOs;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HaushaltsOS.Api.Features.Auth.Login;

/// <summary>
/// Endpoint für die Anmeldung eines Nutzers
/// </summary>
public static class LoginEndpoint
{
    /// <summary>
    /// Registriert die Route für die Benutzeranmeldung
    /// </summary>
    public static void MapLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", HandleAsync);
    }

    private static async Task<IResult> HandleAsync([FromBody] LoginRequest request, UserManager<AppUser> userManager, ITokenService tokenService, CancellationToken cancellationToken)
    {
        // Prüfen, ob ein Konto mit der angegebenen Email Adresse existiert
        AppUser? user = await userManager.FindByEmailAsync(request.Email!);

        if(user is null)
        {
            return Results.Problem(
                title: "Email oder Password ist falsch",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        // Prüfen, ob das Passwort korrekt ist
        bool passwordIsValid = await userManager.CheckPasswordAsync(user!, request.Password!);

        if(!passwordIsValid)
        {
            return Results.Problem(
                title: "Email oder Password ist falsch",
                statusCode: StatusCodes.Status401Unauthorized
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