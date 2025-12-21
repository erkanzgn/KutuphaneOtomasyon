using Kutuphane.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedDate)
            .IsRequired();

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // İlişkiler
        builder.HasMany(u => u.ProcessedLoans)
            .WithOne(l => l.LoanedByUser)
            .HasForeignKey(l => l.LoanedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index'ler
        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Computed property
        builder.Ignore(u => u.FullName);

        builder.HasOne(u => u.Member)
         .WithMany()
         .HasForeignKey(u => u.MemberId)
         .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.Member)
    .WithMany()
    .HasForeignKey(u => u.MemberId)
    .OnDelete(DeleteBehavior.Cascade);
    }
}
