using System.Threading;
using System.Threading.Tasks;

namespace SFTP2.Services.Interfaces
{
    public interface IRunService
    {
        bool HasChanges { get; } // Make it read-only
        Task PerformTaskAsync(CancellationToken stoppingToken);
        void NotifyChange(); // Method to indicate a change has occurred
    }
}