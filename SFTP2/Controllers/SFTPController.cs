using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFTP2.Data;
using SFTP2.Services;
using System.Text.Json.Serialization;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class SFTPController : ControllerBase
{
    private readonly SftpService _sftpService;
    private readonly ILogger<LogFileCleanupService> _logger; // Keeping the original logger
    private readonly ApplicationDbContext _context;


    public SFTPController(SftpService sftpService, ILogger<LogFileCleanupService> logger, ApplicationDbContext context
    )
    {
        _sftpService = sftpService;
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    [Route("getconfig")]
    public IActionResult GetData()
    {
        var data = _context.InFlows
            .Include(inFlow => inFlow.OutFlow)
            //.ThenInclude(outFlow => outFlow.InFlow)
            .FirstOrDefault();

        //var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions
        //{
        //    ReferenceHandler = ReferenceHandler.Preserve,
        //    WriteIndented = true
        //});

        var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        });
        string archivespath = data.ArchivePath;
        return Ok(archivespath);
    }

    [HttpPost("download")]
    public IActionResult DownloadFiles()
    {
        _logger.LogError("Error during log file cleanup"); // This line logs the error

        return Ok("Files downloaded successfully.");
    }
}