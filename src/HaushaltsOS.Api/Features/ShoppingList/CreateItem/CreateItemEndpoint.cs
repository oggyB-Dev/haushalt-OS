using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.DTOs;
using HaushaltsOS.Api.Common.Persistence;
using HaushaltsOS.Api.Common.Realtime;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HaushaltsOS.Api.Features.ShoppingList.CreateItem;

/// <summary>
/// Endpoint zum erstellen eines Artikels
/// </summary>
public static class CreateItemEndpoint
{
    /// <summary>
    /// Registriert die Route für die Artikelerstellung
    /// </summary>
    public static void MapCreateItem(this IEndpointRouteBuilder app)
    {
        app.MapPost("/shopping-items", HandleAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync([FromBody] CreateItemRequest request, AppDbContext dbContext, CurrentUser currentUser, IHubContext<ShoppingListHub> hub,CancellationToken cancellationToken)
    {
        // Prüfen ob der Artikel bereits in der Einkaufsliste vorhanden ist
        bool exists = await dbContext.ShoppingItems
            .AnyAsync(x => x.HouseholdId == currentUser.HouseholdId
                && x.Name == request.Name
                && !x.IsChecked, cancellationToken
            );
        
        if(exists)
        {
            return Results.Problem(
                title: "Artikel ist bereits in der Einkaufsliste",
                statusCode: StatusCodes.Status409Conflict
            );
        }

        // Dto auf Entity mappen
        ShoppingItem item = new ShoppingItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name!,
            Category = request.Category!.Value,
            HouseholdId = currentUser.HouseholdId,
            IsChecked = false,
            SortOrder = 0,
            CreatedAtUtc = DateTime.UtcNow
        };

        // In der Datenbank speichern
        dbContext.ShoppingItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        ShoppingItemResponse response = new ShoppingItemResponse(
            item.Id,
            item.Name,
            item.Category,
            item.SortOrder,
            item.IsChecked,
            item.CheckedByUserId,
            item.CheckedAtUtc
        );

        // Den neuen Artikel an alle Clients im Haushalt senden
        await hub.Clients
            .Group(ShoppingListHub.GroupName(currentUser.HouseholdId))
            .SendAsync("ItemAdded", response, cancellationToken);

        return Results.Created($"/shopping-items/{item.Id}",response);
    }
}