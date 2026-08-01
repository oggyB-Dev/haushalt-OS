using HaushaltsOS.Api.Features.ShoppingList;

namespace HaushaltsOS.Api.Common.DTOs;

/// <summary>
/// Antwort nach erfolgreicher Erstellung eines Artikel
/// </summary>
public sealed record ShoppingItemResponse(
    Guid Id,
    string Name,
    ShoppingListCategory Category,
    int SortOrder,
    bool IsChecked,
    Guid? CheckedByUserId,
    DateTime? CheckedAtUtc
);