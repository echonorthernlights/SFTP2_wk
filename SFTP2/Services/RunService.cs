using System;
using System.Threading;
using System.Threading.Tasks;
using SFTP2.Services.Interfaces;

namespace SFTP2.Services
{
    public class RunService : IRunService
    {
        private bool _hasChanges; // Backing field for HasChanges

        private readonly Lazy<MyBackgroundService> _backgroundService;

        public RunService(Lazy<MyBackgroundService> backgroundService)
        {
            _backgroundService = backgroundService;
            _hasChanges = false; // Initialize as no changes
        }

        public bool HasChanges => _hasChanges; // Read-only property

        public async Task PerformTaskAsync(CancellationToken cancellationToken)
        {
            // Your task logic here
            await Task.Run(() =>
            {
                // Example task logic
                Console.WriteLine("Performing task...");
            }, cancellationToken);
        }

        public void NotifyChange()
        {
            _hasChanges = true; // Set changes to true
        }
    }
}