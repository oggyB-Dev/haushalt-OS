using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.DTOs;
using HaushaltsOS.Api.Common.Persistence;
using HaushaltsOS.Api.Common.Realtime;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HaushaltsOS.Api.Features.ShoppingList.ToggleItem;

/// <summary>
/// Endpoint zum Abhaken eines Artikels
/// </summary>
public static class ToggleItemEndpoint
{
    /// <summary>
    /// Registriert die Route zum Abhaken eines Artikels
    /// </summary>
    public static void MapToggleItem(this IEndpointRouteBuilder app)
    {
        app.MapPost("/shopping-items/{id:guid}/toggle", HandleAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(Guid id, AppDbContext dbContext, CurrentUser currentUser, IHubContext<ShoppingListHub> hub,CancellationToken cancellationToken)
    {
        ShoppingItem? item = await dbContext.ShoppingItems
            .FirstOrDefaultAsync(x => x.Id == id && x.HouseholdId == currentUser.HouseholdId, cancellationToken);
        
        // Prüfen ob der Artikel existiert
        if(item is null)
        {
            return Results.Problem(
                title: "Artikel wurde nicht gefunden",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        // Artikel abhaken oder wieder aktivieren
        item.IsChecked = !item.IsChecked;

        if(item.IsChecked)
        {
            item.CheckedByUserId = currentUser.UserId;
            item.CheckedAtUtc = DateTime.UtcNow;
        }
        else
        {
            item.CheckedByUserId = null;
            item.CheckedAtUtc = null;
        }

        // Änderungen in der Datenbank speichern
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new ShoppingItemResponse(
            item.Id,
            item.Name,
            item.Category,
            item.SortOrder,
            item.IsChecked,
            item.CheckedByUserId,
            item.CheckedAtUtc
        );

        // An alle Clients in der Gruppe senden, dass der Artikel abgehakt wurde
        await hub.Clients
            .Group(ShoppingListHub.GroupName(currentUser.HouseholdId))
            .SendAsync("ItemToggled", response, cancellationToken);
        
        return Results.Ok(response);
    }
}
