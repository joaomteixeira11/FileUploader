using API.Data;
using Microsoft.EntityFrameworkCore;
using API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar o Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpClient("KasperskyClient", client =>
{
    client.BaseAddress = new Uri("https://10.222.56.38:1234");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer nh~~ofa4ES+~cIG-jEXbbJj5p4kQLRKk");
    //client.Timeout = TimeSpan.FromMinutes(5); // We can adjust the timeout as necessary (To resolve a timout problem, it's taking to long to get a answer)
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
    {
        return true; // Ignora a validação do certificado SSL
    }
});

builder.Services.AddScoped<KasperskyService>();

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
