using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.DTOs;
using HaushaltsOS.Api.Common.Persistence;

using Microsoft.EntityFrameworkCore;

namespace HaushaltsOS.Api.Features.Users.GetMe;

/// <summary>
/// Endpoint zum Abfragen des angemeldeten Benutzers
/// </summary>
public static class GetMeEndpoint
{
    /// <summary>
    /// Registriert die Route zum Abfragen des angemeldeten Benutzers
    /// </summary>
    public static void MapGetMe(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me", HandleAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(AppDbContext dbContext, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        UserResponse? user = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == currentUser.UserId)
            .Select(x => new UserResponse(
                x.DisplayName,
                x.Email!
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if(user is null)
        {
            return Results.Problem(
                title: "Benutzer nicht gefunden",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        return Results.Ok(user);
    }
}
