using Microsoft.EntityFrameworkCore;
using OT.Assessment.App.Services;
using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.Consumer.Workers;
using OT.Assessment.Infrastructure;
using OT.Assessment.Infrastructure.Database;
using OT.Assessment.Infrastructure.Interfaces;
using OT.Assessment.Infrastructure.Logging;
using OT.Assessment.Infrastructure.Messaging;
using OT.Assessment.Infrastructure.Repositories;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region Serilog Configure
SerilogConfig.ConfigureLogger();
builder.Host.UseSerilog();
#endregion

#region Database Configure
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));
#endregion

#region Dependencies
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ICasinoWagerRepository, CasinoWagerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRMQConsumer, RMQConsumer>();
//builder.Services.AddSingleton<IRMQConsumer, RMQConsumer>();
builder.Services.AddHostedService<ConsumerWorker>();
#endregion

#region Auto-mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));
#endregion


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        opts.EnableTryItOutByDefault();
        opts.DocumentTitle = "OT Assessment App";
        opts.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting up the API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
