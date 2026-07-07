using HaushaltsOS.Api.Common.Auth;

using Microsoft.AspNetCore.Mvc;

namespace HaushaltsOS.Api.Features.Auth.Refresh;

/// <summary>
/// Endpoint für die Erneuerung der Sitzung
/// </summary>
public static class RefreshEndpoint
{
    // Registriert die Route für die Token Erneuerung
    public static void MapRefresh(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh", HandleAsync);
    }

    private static async Task<IResult> HandleAsync([FromBody] RefreshRequest request,ITokenService tokenService, CancellationToken cancellationToken)
    {
        // Das Refresh Token rotieren
        var token = await tokenService.RotateRefreshTokenAsync(request.RefreshToken!, cancellationToken);

        // Prüfen ob die Rotation erfolgreich war 
        if(token is null)
        {
            return Results.Problem(
                title: "Ungültiges oder abgelaufenes Token.",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        return Results.Ok(token);
    }
}