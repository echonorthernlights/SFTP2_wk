using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SFTPController : ControllerBase
{
    private readonly SftpService _sftpService;

    public SFTPController(SftpService sftpService)
    {
        _sftpService = sftpService;
    }

    [HttpPost("download")]
    public IActionResult DownloadFiles()
    {
        // Define your remote and local directories
        string remoteDirectory = "/"; // Root directory as per your config
        string localDirectory = @"C:\Users\elhas\Desktop\dest";

        // Call the method to download files
        _sftpService.DownloadFiles(remoteDirectory, localDirectory);

        return Ok("Files downloaded successfully.");
    }
}