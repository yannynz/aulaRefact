using RabbitMQ.Client;
public class RabbitMQConnection
{
    private readonly string _connectionString;

    public RabbitMQConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IConnection GetConnection()
    {
        var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };
        return factory.CreateConnection();
    }
}

