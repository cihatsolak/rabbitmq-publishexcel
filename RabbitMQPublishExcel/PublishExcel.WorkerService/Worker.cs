using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PublishExcel.Shared.Models;
using PublishExcel.WorkerService.Models;
using PublishExcel.WorkerService.Services.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PublishExcel.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly RabbitMQWorkerClientService _rabbitMQWorkerClientService;
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<Worker> _logger;
        private IModel _channel;

        public Worker(ILogger<Worker> logger, RabbitMQWorkerClientService rabbitMQWorkerClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitMQWorkerClientService = rabbitMQWorkerClientService;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQWorkerClientService.Connect();

            _channel.BasicQos(
                prefetchSize: 0, //gelen mesajýn boyutu önemli deðil
                prefetchCount: 1, // tek seferde 1 mesaj, bir bir mesajlarý ilet
                global: false
                );

            return base.StartAsync(cancellationToken);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(
                queue: _rabbitMQWorkerClientService.QueueName,
                autoAck: false,
                consumer: consumer
                );

            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);

            string message = Encoding.UTF8.GetString(@event.Body.ToArray());
            var createExcelMessage = JsonSerializer.Deserialize<CreateExcelMessage>(message);

            using var memoryStream = new MemoryStream();

            var dataSet = new DataSet();
            var dataTable = GetTable("vehicles");
            dataSet.Tables.Add(dataTable);

            var xLWorkbook = new XLWorkbook();
            xLWorkbook.Worksheets.Add(dataSet);
            xLWorkbook.SaveAs(memoryStream);


            ByteArrayContent excelByteArrayContent = new ByteArrayContent(memoryStream.ToArray());
            string requestParameterName = "file";
            string fileName = Guid.NewGuid().ToString() + ".xlsx";


            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(excelByteArrayContent, requestParameterName, fileName);


            string serviceUri = string.Concat("http://localhost:17027/api/files?fileId=", createExcelMessage.FileId);
            using var httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.PostAsync(serviceUri, multipartFormDataContent);
            if (response.IsSuccessStatusCode)
            {
                _channel.BasicAck(
                    deliveryTag: @event.DeliveryTag,
                    multiple: false
                    );

                _logger.LogInformation("Excel e dönüþtürme iþlemi baþarýlý!");
            }
            else
            {
                _logger.LogError("Apiden baþarýsýz!");
            }


        }

        private DataTable GetTable(string tableName)
        {
            List<Vehicle> vehicles;

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RabbitMQPublishExcelDBContext>();
                vehicles = context.Vehicles.ToList();
            }

            DataTable dataTable = new DataTable(tableName);

            dataTable.Columns.Add("Marka", typeof(string));
            dataTable.Columns.Add("Model", typeof(string));
            dataTable.Columns.Add("Alt Model", typeof(string));
            dataTable.Columns.Add("Yýl", typeof(int));
            dataTable.Columns.Add("Renk", typeof(string));
            dataTable.Columns.Add("Fiyat", typeof(decimal));

            vehicles.ForEach(vehicle =>
            {
                dataTable.Rows.Add(
                    vehicle.Brand,
                    vehicle.Model,
                    vehicle.SubModel,
                    vehicle.Year,
                    vehicle.Color,
                    vehicle.Price
                    );
            });

            return dataTable;
        }
    }
}
