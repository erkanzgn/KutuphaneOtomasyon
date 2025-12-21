using Kutuphane.Application;
using Kutuphane.Identity.Extensions;
using Kutuphane.Persistence.Extensions;
using Kutuphane.WebUI.Services.EmailService;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Kutuphane.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

        
            builder.Services.AddApplicationServices();
            builder.Services.AddIdentityServices();
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddScoped<IEmailService, SmtpEmailService>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddWebEncoders(o=>
            {
                o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
        
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";    
                options.AccessDeniedPath = "/Account/AccessDenied";
            });
      
            var app = builder.Build();

   
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/NotFound", "?code={0}");
    

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}