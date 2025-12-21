using Kutuphane.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.LoanDate)
            .IsRequired();

        builder.Property(l => l.DueDate)
            .IsRequired();

        builder.Property(l => l.ReturnDate);

        builder.Property(l => l.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(l => l.Notes)
            .HasMaxLength(500);

        builder.Property(l => l.CreatedDate)
            .IsRequired();

        builder.Property(l => l.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // İlişkiler
        builder.HasOne(l => l.Copy)
            .WithMany(c => c.Loans)
            .HasForeignKey(l => l.CopyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.LoanedByUser)
            .WithMany(u => u.ProcessedLoans)
            .HasForeignKey(l => l.LoanedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index'ler
        builder.HasIndex(l => new { l.CopyId, l.Status });
        builder.HasIndex(l => new { l.MemberId, l.Status });
        builder.HasIndex(l => l.DueDate);

        // Computed properties
        builder.Ignore(l => l.IsOverdue);
        builder.Ignore(l => l.OverdueDays);
    }
}
