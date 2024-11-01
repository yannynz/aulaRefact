using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace MyApp.Services
{
    public class EstoqueService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public EstoqueService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "produto_cadastrado", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var produtoInfo = JsonConvert.DeserializeObject<ProdutoMessage>(message);

                // Lógica para atualizar o estoque com produtoInfo
                Console.WriteLine($"Produto {produtoInfo.NomeProduto} cadastrado com quantidade inicial: {produtoInfo.QuantidadeInicial}");
            };

            _channel.BasicConsume(queue: "produto_cadastrado", autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }

    public class ProdutoMessage
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public int QuantidadeInicial { get; set; }
    }
}

