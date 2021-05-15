using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublishExcel.Web.Models.Contexts;
using System.Linq;

namespace PublishExcel.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using var serviceScope = host.Services.CreateScope();
            var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            appDbContext.Database.Migrate();

            if (!appDbContext.Users.Any())
            {
                userManager.CreateAsync(new IdentityUser
                {
                    UserName = "cihat.solak",
                    Email = "cihatsolak@hotmail.com",
                }, "test123").Wait();

                userManager.CreateAsync(new IdentityUser
                {
                    UserName = "test.user",
                    Email = "testuserk@hotmail.com",
                }, "test123").Wait();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
