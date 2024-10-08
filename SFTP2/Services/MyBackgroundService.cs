using Microsoft.Extensions.Hosting;
using SFTP2.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFTP2.Services
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly IRunService _runService;

        public MyBackgroundService(IRunService runService)
        {
            _runService = runService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_runService.HasChanges)
                {
                    // Perform the task if changes are detected
                    await _runService.PerformTaskAsync(stoppingToken);
                }

                // Wait before checking again
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Adjust the interval as needed
            }
        }
    }
}