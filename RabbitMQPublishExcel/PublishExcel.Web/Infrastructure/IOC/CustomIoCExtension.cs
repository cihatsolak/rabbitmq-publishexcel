using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PublishExcel.Web.Models.Contexts;
using PublishExcel.Web.Services.RabbitMQ;
using RabbitMQ.Client;
using System;

namespace PublishExcel.Web.Infrastructure.IOC
{
    public static class CustomIoCExtension
    {
        public static void AddContextAndIdentityConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MSSqlServer"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<AppDbContext>();
        }

        public static void AddRabbitMQConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(implementationFactory => new ConnectionFactory()
            {
                Uri = new Uri(configuration.GetConnectionString("RabbitMQ")),
                DispatchConsumersAsync = true //Asenkron işlem yapacağım
            });

            services.AddSingleton<RabbitMQClientService>();
            services.AddSingleton<RabbitMQPublisher>();
        }
    }
}
