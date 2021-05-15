using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace PublishExcel.Web.Services.RabbitMQ
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        internal string ExchangeName = "ExcelDirectExchange";
        internal string RoutingVehicleExcel = "vehicle-excel-route";
        internal string QueueName = "convert-vehicle-list-excel";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true }) //Zaten bir kanal var ise
                return _channel;

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare( //Exchange oluşturuyorum.
                exchange: ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false
                );

            _channel.QueueDeclare( //Kuyruk oluşturuyorum
                queue: QueueName,
                durable: true,
                exclusive: false, //Başka bir channel'dan bu kuyruğa erişcem
                autoDelete: false,
                arguments: null
                );

            _channel.QueueBind( //Oluşturduğum kuyruğu Exchange'e bind ediyorum.
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: RoutingVehicleExcel,
                arguments: null
                );

            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu.");

            return _channel;
        }

        public void Dispose() //Uygulama kapandığında dispose olacak
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMq ile bağlantı koptu.");
        }
    }
}
