using Kutuphane.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MemberNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Email)
            .HasMaxLength(100);

        builder.Property(m => m.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.Address)
            .HasMaxLength(250);

        builder.Property(m => m.RegistrationDate)
            .IsRequired();

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Notes)
            .HasMaxLength(500);

        builder.Property(m => m.CreatedDate)
            .IsRequired();

        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // İlişkiler
        builder.HasMany(m => m.Loans)
            .WithOne(l => l.Member)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index'ler
        builder.HasIndex(m => m.MemberNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(m => m.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [Email] IS NOT NULL");

        builder.HasIndex(m => m.Phone);

        // Computed column
        builder.Ignore(m => m.FullName);

        builder.HasMany<User>()
    .WithOne(u => u.Member)
    .HasForeignKey(u => u.MemberId)
    .OnDelete(DeleteBehavior.Cascade);
    }
}
