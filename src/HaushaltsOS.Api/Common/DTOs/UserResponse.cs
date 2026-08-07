namespace HaushaltsOS.Api.Common.DTOs;

/// <summary>
/// Antwort nach erfolgreicher Abfrage des angemeldeten Benutzers
/// </summary>
public sealed record UserResponse(
    string DisplayName,
    string Email
);
