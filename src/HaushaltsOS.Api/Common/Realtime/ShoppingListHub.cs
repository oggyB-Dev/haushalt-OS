using HaushaltsOS.Api.Common.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HaushaltsOS.Api.Common.Realtime;

/// <summary>
/// Echtzeitverbindung für die Einkaufsliste 
/// Jede Verbindung gehört zur Gruppe ihres Haushalts,
/// damit Ereignisse nur dort ankommen
/// </summary>
[Authorize]
public sealed class ShoppingListHub(CurrentUser currentUser) : Hub
{
    /// <summary>
    /// Gruppenname für einen Haushalt
    /// </summary>
    /// <returns></returns>
    public static string GroupName(Guid householdId) => $"household:{householdId}";

    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(currentUser.HouseholdId));
        await base.OnConnectedAsync();
    }
}