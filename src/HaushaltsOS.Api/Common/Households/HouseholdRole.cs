namespace HaushaltsOS.Api.Common.Households;

/// <summary>
/// Rolle eines Mitglieds innerhalb eines Haushalts
/// </summary>
public enum HouseholdRole
{
    /// <summary>
    /// Hat den Haushalt erstllt und kann ihn verwalten
    /// </summary>
    Owner = 0,

    /// <summary>
    /// Ist per Einladungscode beigetreten
    /// </summary>
    Member = 1
}