using EmployeeService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeService.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenSessionConfiguration : IEntityTypeConfiguration<RefreshTokenSession>
{
    public void Configure(EntityTypeBuilder<RefreshTokenSession> builder)
    {
        builder.ToTable("refresh_token_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RefreshTokenHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(x => x.RefreshTokenHash)
            .IsUnique();

        builder.Property(x => x.ReplacedByTokenHash)
            .HasMaxLength(128);

        builder.Property(x => x.DeviceInfo)
            .HasMaxLength(512);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(64);

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.RevokedAtUtc);

        builder.Property(x => x.RevokedReason)
            .HasMaxLength(200);

        builder.Property(x => x.ReuseDetectedAtUtc);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
