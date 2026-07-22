namespace HaushaltsOS.Api.Common.Households;

/// <summary>
/// Haushaltsmitglied
/// </summary>
public sealed class HouseholdMember
{
    /// <summary>
    /// ID des Haushalts, dem das Mitglied angehört 
    /// </summary>
    public Guid HouseholdId { get; set; }

    /// <summary>
    /// Id des Benutzers, der Mitglied im Haushalt ist 
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Rolle des Mitglieds
    /// </summary>
    public HouseholdRole Role { get; set; }

    /// <summary>
    /// Beitrittsdatum
    /// </summary>
    public DateTime JoinedAtUtc { get; set; }
}