using Microsoft.EntityFrameworkCore;
using SFTP2.Data.Entities;
using SFTP2.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFTP2.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IRunService _runService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IRunService runService)
            : base(options)
        {
            _runService = runService;
        }

        public DbSet<InFlow> InFlows { get; set; }
        public DbSet<OutFlow> OutFlows { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=LocalDatabase.db"); // SQLite database in a file
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring the one-to-one relationship
            modelBuilder.Entity<InFlow>()
                .HasOne(inFlow => inFlow.OutFlow)
                .WithOne(outFlow => outFlow.InFlow)
                .HasForeignKey<InFlow>(inFlow => inFlow.OutFlowId);

            // Optional: Configure the reverse relationship for the foreign key in OutFlow
            //modelBuilder.Entity<OutFlow>()
            //    .HasOne(outFlow => outFlow.InFlow)
            //    .WithOne(inFlow => inFlow.OutFlow)
            //    .HasForeignKey<OutFlow>(outFlow => outFlow.InFlowId);

            // Seeding initial data with GUIDs
            var outFlowId = Guid.NewGuid().ToString();
            var inFlowId = Guid.NewGuid().ToString();

            modelBuilder.Entity<OutFlow>().HasData(
                new OutFlow
                {
                    Id = outFlowId,
                    ServerAddress = "sftp://example.com",
                    RemotePath = "/remote/path",
                    InFlowId = inFlowId
                }
            );

            modelBuilder.Entity<InFlow>().HasData(
                new InFlow
                {
                    Id = inFlowId,
                    ServerAddress = "sftp://example.com",
                    ArchivePath = "/archive/path",
                    OutFlowId = outFlowId
                }
            );
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var hasChanges = ChangeTracker.HasChanges();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (hasChanges)
            {
                _runService.NotifyChange();
            }

            return result;
        }

        public override int SaveChanges()
        {
            var hasChanges = ChangeTracker.HasChanges();

            var result = base.SaveChanges();

            if (hasChanges)
            {
                _runService.NotifyChange();
            }

            return result;
        }
    }
}
