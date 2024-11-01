using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

    public class RabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(IConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
        }

        public void Publish<T>(T message, string queueName)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }

