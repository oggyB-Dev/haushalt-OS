using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.DTOs;
using HaushaltsOS.Api.Common.Persistence;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HaushaltsOS.Api.Features.Households.GetHousehold;

/// <summary>
/// Endpoint zum abfragen eines Haushalts
/// </summary>
public static class GetHouseholdEndpoint
{
    /// <summary>
    /// Registriert die Route zum abfragen eines Haushalts
    /// </summary>
    public static void MapGetHousehold(this IEndpointRouteBuilder app)
    {
        app.MapGet("/household", HandleAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(AppDbContext dbContext, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        HouseholdResponse? household = await dbContext.Households
            .Where(x => x.Id == currentUser.HouseholdId)
            .Select(x => new HouseholdResponse(
                x.Name,
                x.InviteCode
            ))
            .FirstOrDefaultAsync(cancellationToken);
        
        if(household is null)
        {
            return Results.Problem(
                title: "Haushalt nicht gefunden",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        return Results.Ok(household);
    }
}