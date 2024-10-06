using System.Diagnostics;
using SFTP2.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SFTP2.Services
{
    public class LogFileCleanupService : BackgroundService, ILogFileCleanupService
    {
        private readonly string _logDirectory = "Logging";

        public LogFileCleanupService() { }

        public async Task CleanupAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var logFiles = Directory.EnumerateFiles(_logDirectory, "*.txt");
                    foreach (var file in logFiles)
                    {
                        var creationTime = File.GetCreationTime(file);
                        // Check if the file is older than 24 hours
                        if (DateTime.Now >= creationTime.AddMinutes(1))
                        {
                            Debug.WriteLine($"Attempting to delete file: {file}");
                            UnlockAndDeleteFile(file);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during log file cleanup: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken); // Check every 2 minutes
            }
        }

        private void UnlockAndDeleteFile(string filePath)
        {
            //if (!IsFileLocked(filePath))
            //{
                try
                {
                    // If the file is not locked, delete it
                    File.Delete(filePath);
                    Debug.WriteLine($"Successfully deleted file: {filePath}");
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"Failed to delete file {filePath}: {ex.Message}.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error while trying to delete file: {filePath}, error: {ex.Message}");
                }
            //}
            //else
            //{
            //    Debug.WriteLine($"File {filePath} is currently locked. Skipping deletion.");
            //}
        }

        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // If successful, the file is not locked
                }
            }
            catch (IOException)
            {
                // The file is locked
                return true;
            }

            return false; // The file is not locked
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await CleanupAsync(stoppingToken);
        }
    }
}
