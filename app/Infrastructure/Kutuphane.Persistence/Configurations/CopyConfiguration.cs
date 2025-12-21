using Kutuphane.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Configurations;

public class CopyConfiguration : IEntityTypeConfiguration<Copy>
{
    public void Configure(EntityTypeBuilder<Copy> builder)
    {
        builder.ToTable("Copies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CopyNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.ShelfLocation)
            .HasMaxLength(50);

        builder.Property(c => c.AcquisitionDate)
            .IsRequired();

        builder.Property(c => c.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Condition)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedDate)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // İlişkiler
        builder.HasOne(c => c.Book)
            .WithMany(b => b.Copies)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Loans)
            .WithOne(l => l.Copy)
            .HasForeignKey(l => l.CopyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index'ler
        builder.HasIndex(c => new { c.BookId, c.CopyNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(c => c.Status);
    }
}
