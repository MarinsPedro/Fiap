using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Entities;
using FiapCloudGames.Promotions.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FiapCloudGames.Promotions.Infrastructure.Persistence;

public sealed class PromotionsDbContext(DbContextOptions<PromotionsDbContext> options)
    : DbContext(options), IPromotionsUnitOfWork
{
    public DbSet<Promotion> Promotions => Set<Promotion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var discountConverter =
            new ValueConverter<DiscountPercentage, decimal>(
                discount => discount.Value,
                value => DiscountPercentage.Create(value));

        modelBuilder.HasDefaultSchema("promotions");
        modelBuilder.Entity<Promotion>(builder =>
        {
            builder.ToTable("promotions");
            builder.HasKey(promotion => promotion.Id);
            builder.Property(promotion => promotion.Id).HasColumnName("id").ValueGeneratedNever();
            builder.Property(promotion => promotion.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
            builder.Property(promotion => promotion.DiscountPercent)
                .HasColumnName("discount_percent")
                .HasConversion(discountConverter)
                .HasPrecision(5, 2)
                .IsRequired();
            builder.Property(promotion => promotion.StartsAtUtc).HasColumnName("starts_at_utc").IsRequired();
            builder.Property(promotion => promotion.EndsAtUtc).HasColumnName("ends_at_utc").IsRequired();
            builder.Property(promotion => promotion.EndedAtUtc).HasColumnName("ended_at_utc");
            builder.Property(promotion => promotion.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
            builder.HasMany(promotion => promotion.Games)
                .WithOne()
                .HasForeignKey(item => item.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(promotion => promotion.Games).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.HasIndex(promotion => new { promotion.StartsAtUtc, promotion.EndsAtUtc });
        });

        modelBuilder.Entity<PromotionGame>(builder =>
        {
            builder.ToTable("promotion_games");
            builder.HasKey(item => new { item.PromotionId, item.GameId });
            builder.Property(item => item.PromotionId).HasColumnName("promotion_id");
            builder.Property(item => item.GameId).HasColumnName("game_id");
            builder.HasIndex(item => item.GameId);
        });
    }
}
