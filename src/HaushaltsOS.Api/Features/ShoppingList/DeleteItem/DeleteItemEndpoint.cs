using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.Persistence;
using HaushaltsOS.Api.Common.Realtime;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HaushaltsOS.Api.Features.ShoppingList.DeleteItem;

/// <summary>
/// Endpoint zum Löschen eines Artikels
/// </summary>
public static class DeleteItemEndpoint
{
    /// <summary>
    /// Registriert die Route zum Löschen eines Artikels
    /// </summary>
    public static void MapDeleteItem(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/shopping-items/{id:guid}", HandleAsync)
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

        dbContext.ShoppingItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        // An alle Clients in der Gruppe senden, dass der Artikel gelöscht wurde
        await hub.Clients
            .Group(ShoppingListHub.GroupName(currentUser.HouseholdId))
            .SendAsync("ItemDeleted", item.Id, cancellationToken);

        return Results.NoContent();
    }
}