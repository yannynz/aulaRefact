using MyApp.Repositories;
using MyApp.Services;
using web_app_repository;
using MySqlConnector;
using Dapper;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar RabbitMQ Connection
var rabbitMQConnectionString = "amqp://localhost"; // Altere conforme necessário
builder.Services.AddSingleton<RabbitMQConnection>(sp => new RabbitMQConnection(rabbitMQConnectionString));
builder.Services.AddSingleton<IConnection>(sp => sp.GetRequiredService<RabbitMQConnection>().GetConnection());
builder.Services.AddSingleton<RabbitMQService>(); // Adicionando RabbitMQService como singleton

// Configurar Redis Connection
var redisConnectionString = "localhost:6379"; // Altere conforme necessário
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

// Outras configurações
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Injeção de dependência
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection("Server=localhost;Database=sys;User=root;Password=123;"));

// Política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usando a política CORS
app.UseCors("AllowAllOrigins");

app.UseAuthorization();
app.MapControllers();
app.Run();

