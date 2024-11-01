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
var rabbitMQConnectionString = "amqp://localhost"; // Altere conforme necess�rio
builder.Services.AddSingleton<RabbitMQConnection>(sp => new RabbitMQConnection(rabbitMQConnectionString));
builder.Services.AddSingleton<IConnection>(sp => sp.GetRequiredService<RabbitMQConnection>().GetConnection());
builder.Services.AddSingleton<RabbitMQService>(); // Adicionando RabbitMQService como singleton

// Configurar Redis Connection
var redisConnectionString = "localhost:6379"; // Altere conforme necess�rio
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

// Outras configura��es
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inje��o de depend�ncia
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection("Server=localhost;Database=sys;User=root;Password=123;"));

// Pol�tica de CORS
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

// Usando a pol�tica CORS
app.UseCors("AllowAllOrigins");

app.UseAuthorization();
app.MapControllers();
app.Run();

