using Kutuphane.Domain.Common;
using Kutuphane.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Context;

public class KutuphaneDbContext:DbContext
{
    public KutuphaneDbContext(DbContextOptions<KutuphaneDbContext> options):base(options)
    {

    }
    public DbSet<Book> Books { get; set; }
    public DbSet<Copy> Copies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Book>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Copy>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Loan>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Member>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<ContactMessage>().HasQueryFilter(x => !x.IsDeleted);

  
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
     
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.Now;
                entry.Entity.IsDeleted = false;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedDate = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    private void SeedData(ModelBuilder modelBuilder)
    {
      
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@kutuphane.com",
            PasswordHash = "$2a$11$8ZqXH5Z3bZN9cY7wX8vGVeK3pX6L9mN5qR7tU9vW1xY3zA5bC7dE.", // Admin123!
            FirstName = "Admin",
            LastName = "User",
            Role = Domain.Enums.UserRole.Admin,
            IsActive = true,
            CreatedDate = DateTime.Now,
            IsDeleted = false
        });
    }
}
