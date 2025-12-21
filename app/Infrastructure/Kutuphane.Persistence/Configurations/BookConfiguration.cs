using Kutuphane.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(t => t.Id);

        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(13);

        builder.Property(b => b.Title)
         .IsRequired()
         .HasMaxLength(200);

        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Publisher)
            .HasMaxLength(100);

        builder.Property(b => b.Category)
            .HasMaxLength(50);

        builder.Property(b => b.Language)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Türkçe");

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.CreatedDate)
            .IsRequired();

        builder.Property(b => b.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // İlişkiler
        builder.HasMany(b => b.Copies)
            .WithOne(c => c.Book)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Restrict); // Kopya varsa kitap silinemez

        // Index'ler
        builder.HasIndex(b => b.ISBN)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0"); // Soft delete için
    }
}
