namespace HaushaltsOS.Api.Common.DTOs;

/// <summary>
/// Antwort nach erfolgreicher Abfrage eines Haushalts
/// </summary>
public sealed record HouseholdResponse(
    string Name,
    string InviteCode
);