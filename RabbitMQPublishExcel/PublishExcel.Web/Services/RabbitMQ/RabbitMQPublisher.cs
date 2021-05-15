
using PublishExcel.Shared.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PublishExcel.Web.Services.RabbitMQ
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(CreateExcelMessage createExcelMessage)
        {
            IModel channel = _rabbitMQClientService.Connect();

            string message = JsonSerializer.Serialize(createExcelMessage);
            byte[] bodyByteMessage = Encoding.UTF8.GetBytes(message);

            IBasicProperties basicProperties = channel.CreateBasicProperties();  //Mesajım rabbitmq memory'de değil fiziksel diskde tutulsun
            basicProperties.Persistent = true; //Mesajım rabbitmq memory'de değil fiziksel diskde tutulsun

            channel.BasicPublish(
                exchange: _rabbitMQClientService.ExchangeName, //Exchange adı
                routingKey: _rabbitMQClientService.RoutingVehicleExcel, //RouteKey
                basicProperties: basicProperties,
                body: bodyByteMessage
                );
        }
    }
}
