namespace SFTP2.Services.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILogFileCleanupService
    {
        Task CleanupAsync(CancellationToken stoppingToken);
    }
}