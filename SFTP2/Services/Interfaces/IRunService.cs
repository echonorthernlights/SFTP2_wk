namespace SFTP2.Services.Interfaces
{
    public interface IRunService
    {
        bool HasChanges { get; set; }
        Task PerformTaskAsync(CancellationToken stoppingToken);
        void NotifyChange();
    }
}