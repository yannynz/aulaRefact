using web_app_repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configurar inje��o de depend�ncia
//Voc�s v�o esquecer de fazer isso e dar� erro! 
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
var app = builder.Build();
builder.Services.AddScoped<IProdutoRepository>(sp => new ProdutoRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
