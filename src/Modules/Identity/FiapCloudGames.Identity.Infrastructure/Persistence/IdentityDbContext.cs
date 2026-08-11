using FiapCloudGames.Identity.Application.Abstractions.Persistence;
using FiapCloudGames.Identity.Domain.Entities;
using FiapCloudGames.Identity.Domain.Enums;
using FiapCloudGames.Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FiapCloudGames.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : DbContext(options), IIdentityUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var emailConverter = new ValueConverter<Email, string>(
            email => email.Value,
            value => Email.Create(value));

        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("users");
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Id).HasColumnName("id");
            builder.Property(user => user.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
            builder.Property(user => user.Email).HasColumnName("email").HasMaxLength(254)
                .HasConversion(emailConverter).IsRequired();
            builder.HasIndex(user => user.Email).IsUnique();
            builder.Property(user => user.PasswordHash).HasColumnName("password_hash").HasMaxLength(500).IsRequired();
            builder.Property(user => user.Role).HasColumnName("role").HasConversion<int>()
                .HasDefaultValue(UserRole.User).IsRequired();
            builder.Property(user => user.IsActive).HasColumnName("is_active").IsRequired();
            builder.Property(user => user.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        });
    }

}
