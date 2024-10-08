using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SFTP2.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SFTP2.Data;

namespace SFTP2.Services
{
    public class RunService : IRunService, IHostedService
    {
        private readonly ApplicationDbContext _context;
        private readonly GlobalDataService _globalDataService;
        private bool _hasChanges;
        private Timer _timer;

        public RunService(ApplicationDbContext context, GlobalDataService globalDataService)
        {
            _context = context;
            _globalDataService = globalDataService;
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set => _hasChanges = value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            PerformTaskAsync(CancellationToken.None).Wait();
        }

        public async Task PerformTaskAsync(CancellationToken stoppingToken)
        {
            if (_hasChanges)
            {
                var data = await _context.InFlows
                    .Include(inFlow => inFlow.OutFlow)
                    .FirstOrDefaultAsync(stoppingToken);

                if (data != null)
                {
                    var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        WriteIndented = true
                    });

                    string archivesPath = data.ArchivePath;

                    // Update the global data
                    _globalDataService.Data = data;

                    // You can now use archivesPath as needed
                    // For example, log it or perform other operations
                }

                // After performing the task, reset the flag
                _hasChanges = false;
            }
        }

        public void NotifyChange()
        {
            _hasChanges = true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
