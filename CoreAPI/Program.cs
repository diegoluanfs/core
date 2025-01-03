using CoreAPI.Config;
using CoreAPI.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog para logs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

Log.Information("A aplicação está sendo inicializada...");

// Adicionar serviços ao contêiner
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<UserRepository>();

// Configuração de autenticação (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourissuer",
            ValidAudience = "youraudience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("your_secret_key"))
        };
    });

builder.Services.AddAuthorization();

// Adicionar controladores e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar o pipeline de requisição
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Rota Default para a Página Inicial
app.MapGet("/", () =>
{
    Log.Information("Endpoint '/' acessado - Aplicação está rodando.");
    return Results.Ok(new { message = "Bem-vindo à CoreAPI! A aplicação está rodando." });
});

// Mapear controladores
app.MapControllers();

// Log para indicar que a aplicação subiu
Log.Information("A aplicação subiu com sucesso e está pronta para receber requisições.");

// Iniciar a aplicação
app.Run();
