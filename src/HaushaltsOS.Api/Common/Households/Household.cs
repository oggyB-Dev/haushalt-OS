namespace HaushaltsOS.Api.Common.Households;

/// <summary>
/// Haushalt
/// </summary>
public sealed class Household
{
    /// <summary>
    /// ID des Haushalts
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Der Anzeigename des Haushalts
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Eindeutiger Einladungscode
    /// </summary>
    public required string InviteCode { get; set; } 

    /// <summary>
    /// Erstellungsdatum
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
}