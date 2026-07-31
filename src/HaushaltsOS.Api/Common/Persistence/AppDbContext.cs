using HaushaltsOS.Api.Common.Auth;
using HaushaltsOS.Api.Common.Households;
using HaushaltsOS.Api.Features.ShoppingList;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HaushaltsOS.Api.Common.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) 
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    /// <summary>
    /// Gespeicherte Refresh Tokens
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// Artikel der Einkaufsliste
    /// </summary>
    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();

    /// <summary>
    /// Haushalte
    /// </summary>
    public DbSet<Household> Households => Set<Household>();

    /// <summary>
    /// Haushaltsmitgliedschaften
    /// </summary>
    public DbSet<HouseholdMember> HouseholdMembers => Set<HouseholdMember>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Ein Benutzer ist pro Haushalt einmal Mitglied
        builder.Entity<HouseholdMember>()
            .HasKey(x => new {x.HouseholdId, x.UserId});

        // Einladungscodes müssen eindeutig sein
        builder.Entity<Household>()
            .HasIndex(x => x.InviteCode)
            .IsUnique();
        
        // Alle Artikel eines Haushalts
        builder.Entity<ShoppingItem>()
            .HasIndex(x => x.HouseholdId);
    }
}