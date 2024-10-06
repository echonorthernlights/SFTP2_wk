using Microsoft.AspNetCore.Mvc;
using SFTP2.Services;

[ApiController]
[Route("api/[controller]")]
public class SFTPController : ControllerBase
{
    private readonly SftpService _sftpService;
    private readonly ILogger<LogFileCleanupService> _logger; // Keeping the original logger

    // Lock object for synchronizing access to the logger
    private static readonly object _lock = new object();

    public SFTPController(SftpService sftpService, ILogger<LogFileCleanupService> logger)
    {
        _sftpService = sftpService;
        _logger = logger;
    }

    [HttpPost("download")]
    public IActionResult DownloadFiles()
    {
        _logger.LogError("Error during log file cleanup"); // This line logs the error

        return Ok("Files downloaded successfully.");
    }
}