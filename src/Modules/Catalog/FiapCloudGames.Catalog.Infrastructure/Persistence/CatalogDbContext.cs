using FiapCloudGames.Catalog.Application.Abstractions.Persistence;
using FiapCloudGames.Catalog.Domain.Entities;
using FiapCloudGames.Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FiapCloudGames.Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options)
    : DbContext(options), ICatalogUnitOfWork
{
    public DbSet<Game> Games => Set<Game>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var priceConverter =
            new ValueConverter<GamePrice, decimal>(
                price => price.Amount,
                value => GamePrice.Create(value));

        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.Entity<Game>(builder =>
        {
            builder.ToTable("games");
            builder.HasKey(game => game.Id);
            builder.Property(game => game.Id).HasColumnName("id");
            builder.Property(game => game.Title).HasColumnName("title").HasMaxLength(160).IsRequired();
            builder.Property(game => game.Description).HasColumnName("description").HasMaxLength(4000).IsRequired();
            builder.Property(game => game.Category).HasColumnName("category").HasMaxLength(80).IsRequired();
            builder.Property(game => game.BasePrice)
                .HasColumnName("base_price")
                .HasConversion(priceConverter)
                .HasPrecision(12, 2)
                .IsRequired();
            builder.Property(game => game.IsActive).HasColumnName("is_active").IsRequired();
            builder.Property(game => game.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            builder.HasIndex(game => game.Title);
        });
    }
}
