using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using SFTP2.Data;
using SFTP2.Services;
using SFTP2.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings.json
var logFileName = $"logs_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.txt";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File($"Logging/{logFileName}",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Minute,
        retainedFileCountLimit: 10,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
        shared: true)
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the ApplicationDbContext with dependency injection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=LocalDatabase.db")); // SQLite database in a file

// Register MyBackgroundService as a singleton
builder.Services.AddSingleton<MyBackgroundService>();

// Register IRunService with Lazy<MyBackgroundService>
builder.Services.AddScoped<IRunService>(provider =>
{
    var backgroundService = new Lazy<MyBackgroundService>(() => provider.GetRequiredService<MyBackgroundService>());
    return new RunService(backgroundService);
});

// Register additional services
builder.Services.AddScoped<DbChangeNotifierService>();
builder.Services.AddScoped<SftpService>();
builder.Services.AddHostedService<LogFileCleanupService>();
builder.Services.AddHostedService<MyBackgroundService>(); // This should ideally be the same singleton service

// Global config
builder.Services.AddSingleton<GlobalDataService>();

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
