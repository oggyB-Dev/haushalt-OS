namespace HaushaltsOS.Api.Features.ShoppingList;

/// <summary>
/// Artikel für die Einkaufsliste
/// </summary>
public sealed class ShoppingItem
{
    /// <summary>
    /// Primary Key
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Foreign Key Haushalt
    /// </summary>
    public Guid HouseholdId { get; init; }

    /// <summary>
    /// Name des Artikels
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Artikelkategorie
    /// </summary>
    public ShoppingListCategory Category { get; set; }

    /// <summary>
    /// Reihenfolge für die Anzeige innerhalb der Einkaufsliste
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Flag ob es gekauft wurde oder nicht 
    /// </summary>
    public bool IsChecked { get; set; }
    
    /// <summary>
    /// Id des Nutzers der gekauft hat
    /// </summary>
    public Guid? CheckedByUserId { get; set; }

    /// <summary>
    /// Datum des Einkaufs
    /// </summary>
    public DateTime? CheckedAtUtc { get; set; }

    /// <summary>
    /// Zeitpunkt zu dem der Artikel angelegt wurde
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

}