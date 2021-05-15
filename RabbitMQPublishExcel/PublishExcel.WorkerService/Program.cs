using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublishExcel.WorkerService.Services.RabbitMQ;
using RabbitMQ.Client;
using System;

namespace PublishExcel.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    #region DI Services
                    services.AddSingleton(implementationFactory => new ConnectionFactory()
                    {
                        Uri = new Uri(configuration.GetConnectionString("RabbitMQ")),
                        DispatchConsumersAsync = true //Asenkron iþlem yapacaðým
                    });

                    services.AddSingleton<RabbitMQClientService>();
                    #endregion

                    #region Hosted Services
                    services.AddHostedService<Worker>();
                    #endregion
                });
    }
}
