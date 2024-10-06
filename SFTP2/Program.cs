using Microsoft.Extensions.Logging;
using Serilog;
using SFTP2.Services;
using SFTP2.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings.json
//var logFileName = $"logs_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.txt";
//Log.Logger = new LoggerConfiguration()
//    .WriteTo.File($"Logging/{logFileName}", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
//        rollingInterval: RollingInterval.Day,
//        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error // Only log errors
//        )

//    .ReadFrom.Configuration(builder.Configuration)
//    .CreateLogger();

var logFileName = $"logs_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.txt";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File($"Logging/{logFileName}",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Minute,
        retainedFileCountLimit: 10,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
        shared:true)
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();


builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register the SFTP service and the log file cleanup service
builder.Services.AddScoped<SftpService>();
builder.Services.AddHostedService<LogFileCleanupService>();
//builder.Services.AddScoped<ILogFileCleanupService, LogFileCleanupService>();


var app = builder.Build();

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