using System.ComponentModel.DataAnnotations;

namespace HaushaltsOS.Api.Features.ShoppingList.CreateItem;

/// <summary>
/// Anfrage zum erstellen eines Artikels
/// </summary>
public sealed class CreateItemRequest
{
    /// <summary>
    /// Artikelname
    /// </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string? Name { get; set; }
    
    /// <summary>
    /// Artikelkategorie
    /// </summary>
    [Required]
    public ShoppingListCategory? Category { get; set; }
}