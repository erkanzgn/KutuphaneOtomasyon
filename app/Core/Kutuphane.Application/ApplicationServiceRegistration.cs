using Kutuphane.Application.Interfaces.Services;
using Kutuphane.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ICopyService, CopyService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactService, ContactService>();

            return services;
        }
    }
}
