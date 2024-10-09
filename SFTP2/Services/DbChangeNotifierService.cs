using Microsoft.EntityFrameworkCore;
using SFTP2.Data;
using SFTP2.Data.Entities;
using System.Threading;
using System.Threading.Tasks;
using SFTP2.Services.Interfaces;

namespace SFTP2.Services
{
    public class DbChangeNotifierService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRunService _runService;

        public DbChangeNotifierService(ApplicationDbContext context, IRunService runService)
        {
            _context = context;
            _runService = runService;
        }

        public async Task<InFlow> GetInFlowWithOutFlowAsync(CancellationToken cancellationToken)
        {
            return await _context.InFlows
                .Include(inFlow => inFlow.OutFlow)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var hasChanges = _context.ChangeTracker.HasChanges();
            var result = await _context.SaveChangesAsync(cancellationToken);

            if (hasChanges)
            {
                _runService.NotifyChange();
            }

            return result;
        }

        public int SaveChanges()
        {
            var hasChanges = _context.ChangeTracker.HasChanges();
            var result = _context.SaveChanges();

            if (hasChanges)
            {
                _runService.NotifyChange();
            }

            return result;
        }
    }
}