using Kutuphane.Application.Interfaces.Repositories;
using Kutuphane.Persistence.Context;
using Kutuphane.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Persistence.Extensions;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<KutuphaneDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(KutuphaneDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICopyRepository, CopyRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();

        return services;
    }
}
