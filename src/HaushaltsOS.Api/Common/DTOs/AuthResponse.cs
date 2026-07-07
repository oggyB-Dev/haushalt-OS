namespace HaushaltsOS.Api.Common.DTOs;

/// <summary>
/// Antwort nach erfolgreicher Registrierung oder Anmeldung
/// </summary>
/// <param name="AccessToken"></param>
/// <param name="RefreshToken"></param>
public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken
);