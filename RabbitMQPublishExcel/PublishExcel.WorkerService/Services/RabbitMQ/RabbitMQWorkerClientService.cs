using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace PublishExcel.WorkerService.Services.RabbitMQ
{
    public class RabbitMQWorkerClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        private readonly ILogger<RabbitMQWorkerClientService> _logger;

        internal string QueueName = "convert-vehicle-list-excel";

        public RabbitMQWorkerClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQWorkerClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        /// <summary>
        /// Publisher tarafında exchange oluşturdum, kuyruk oluşturdum ve kuyruğu bind ettiğim için consumer yani bu tarafta bu işleri yapmamama gerek yok. Bunun bir sebebide direct exchange kullanıyor olmam.
        /// BasicConsume metotuyla dinlemek istediğim kuyruk ismi yeterli.
        /// Direct Exchange belirlediğim bir konuma mesajları depolar.
        /// </summary>
        /// <returns></returns>
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true }) //Zaten bir kanal var ise
                return _channel;

            _channel = _connection.CreateModel();
                           
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
