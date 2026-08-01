using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.DTOs;
using HaushaltsOS.Api.Common.Persistence;

using Microsoft.EntityFrameworkCore;

namespace HaushaltsOS.Api.Features.ShoppingList.GetItems;

/// <summary>
/// Endpoint zum abrufen der Artikel
/// </summary>
public static class GetItemsEndpoint
{
    /// <summary>
    /// Registriert die Route für den Abruf der Artikel
    /// </summary>
    /// <param name="app"></param>
    public static void MapGetItems(this IEndpointRouteBuilder app)
    {
        app.MapGet("/shopping-items", HandleAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(AppDbContext dbContext, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        List<ShoppingItemResponse>? items = await dbContext.ShoppingItems
            .AsNoTracking()
            .Where(x => x.HouseholdId == currentUser.HouseholdId)
            .OrderBy(x => x.IsChecked)
            .ThenBy(x => x.Category)
            .ThenBy(x => x.SortOrder)
            .Select(x => new ShoppingItemResponse(
                x.Id,
                x.Name,
                x.Category,
                x.SortOrder,
                x.IsChecked,
                x.CheckedByUserId,
                x.CheckedAtUtc
            )).ToListAsync(cancellationToken);

        
        return Results.Ok(items);
    }


}