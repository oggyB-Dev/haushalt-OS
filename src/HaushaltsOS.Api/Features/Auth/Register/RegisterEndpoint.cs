using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.DTOs;
using HaushaltsOS.Api.Common.Households;
using HaushaltsOS.Api.Common.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaushaltsOS.Api.Features.Auth.Register;

/// <summary>
/// Endpoint für die Registrierung eines neuen Benutzers
/// </summary>
public static class RegisterEndpoint
{
    /// <summary>
    /// Registriert die Route für die Benutzerregistrierung
    /// </summary>
    public static void MapRegister(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", HandleAsync);
    }

    private static async Task<IResult> HandleAsync([FromBody] RegisterRequest request, AppDbContext dbContext,UserManager<AppUser> userManager, ITokenService tokenService, CancellationToken cancellationToken)
    {
        AppUser? existing = await userManager.FindByEmailAsync(request.Email!);

        if(existing is not null)
        {
            return Results.Problem(
                title: "Email bereits vergeben",
                statusCode: StatusCodes.Status409Conflict
            );
        }

        Household? householdToJoin = null;

        if(!string.IsNullOrWhiteSpace(request.InviteCode))
        {
            householdToJoin = await dbContext.Households
                .FirstOrDefaultAsync(x => x.InviteCode == request.InviteCode, cancellationToken);

            if(householdToJoin is null)
            {
                return Results.Problem(
                    title: "Ungültiger Einladungscode.",
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        AppUser user = new()
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName!
        };
        
        // Passwort hashen und User speichern
        IdentityResult result = await userManager.CreateAsync(user, request.Password!);

        if(!result.Succeeded)
        {
            return Results.Problem(
                title: "Registrierung fehlgeschlagen",
                detail: string.Join("; ", result.Errors.Select(err => err.Description)),
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        HouseholdRole role = HouseholdRole.Member;

        if(householdToJoin is null)
        {
            householdToJoin = new Household
            {
                Id = Guid.NewGuid(),
                Name = $"Haushalt von {request.DisplayName}",
                InviteCode = InviteCodeGenerator.Generate(),
                CreatedAtUtc = DateTime.UtcNow
            };
            dbContext.Households.Add(householdToJoin);
            role = HouseholdRole.Owner;
        }

        dbContext.HouseholdMembers.Add(new HouseholdMember
        {
            HouseholdId = householdToJoin.Id,
            UserId = user.Id,
            Role = role,
            JoinedAtUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        string accessToken = await tokenService.CreateAccessTokenAsync(user, cancellationToken);
        string refreshToken = await tokenService.CreateRefreshTokenAsync(user, cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return Results.Ok(new AuthResponse(
            accessToken,
            refreshToken
        ));

    }
}