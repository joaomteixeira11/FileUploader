using API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar o Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpClient("KasperskyClient", client =>
{
    client.BaseAddress = new Uri("https://10.222.56.38:1234");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer nh~~ofa4ES+~cIG-jEXbbJj5p4kQLRKk");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
    {
        return true; // Ignora a validação do certificado SSL
    }
});

builder.Services.AddScoped<IKasperskyService, KasperskyService>();

// Configurar o Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy.WithOrigins("http://localhost:4201", "https://localhost:4201") // URL frontend Angular
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

var app = builder.Build();

// HTTP request pipeline.
app.UseCors("AllowSpecificOrigin");

app.UseRouting();

app.MapControllers();

app.Run();
