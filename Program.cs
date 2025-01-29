using Gadaxede;
using Gadaxede.Data;
using Gadaxede.Interfaces;
using Gadaxede.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<Seed>();
builder.Services.AddControllers();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<IWebSocketRepository, WebSocketRepository>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy =>
                      {
                          policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
});
});
var app = builder.Build();

// Enable WebSockets
app.UseWebSockets();
app.UseCors();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.Initialize();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();     //uncomment if u want https

app.UseAuthorization();

// Map WebSocketController here (you can create this controller)
app.MapControllers();

app.Run();
