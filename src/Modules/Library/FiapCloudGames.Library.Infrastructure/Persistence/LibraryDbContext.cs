using FiapCloudGames.Library.Application.Abstractions.Persistence;
using FiapCloudGames.Library.Domain.Entities;
using FiapCloudGames.Library.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FiapCloudGames.Library.Infrastructure.Persistence;

public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options)
    : DbContext(options), ILibraryUnitOfWork
{
    public DbSet<GameLibrary> Libraries => Set<GameLibrary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var priceConverter =
            new ValueConverter<AcquisitionPrice, decimal>(
                price => price.Amount,
                value => AcquisitionPrice.Create(value));

        modelBuilder.HasDefaultSchema("library");
        modelBuilder.Entity<GameLibrary>(builder =>
        {
            builder.ToTable("game_libraries");
            builder.HasKey(library => library.Id);
            builder.Property(library => library.Id).HasColumnName("id");
            builder.Property(library => library.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(library => library.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            builder.HasIndex(library => library.UserId).IsUnique();
            builder.HasMany(library => library.Games)
                .WithOne()
                .HasForeignKey(item => item.LibraryId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(library => library.Games).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<LibraryGame>(builder =>
        {
            builder.ToTable("library_games");
            builder.HasKey(item => item.Id);
            builder.Property(item => item.Id).HasColumnName("id");
            builder.Property(item => item.LibraryId).HasColumnName("library_id").IsRequired();
            builder.Property(item => item.GameId).HasColumnName("game_id").IsRequired();
            builder.Property(item => item.PricePaid)
                .HasColumnName("price_paid")
                .HasConversion(priceConverter)
                .HasPrecision(12, 2)
                .IsRequired();
            builder.Property(item => item.PromotionId).HasColumnName("promotion_id");
            builder.Property(item => item.AcquiredAtUtc).HasColumnName("acquired_at_utc").IsRequired();
            builder.HasIndex(item => new { item.LibraryId, item.GameId }).IsUnique();
            builder.HasIndex(item => item.GameId);
        });
    }
}
