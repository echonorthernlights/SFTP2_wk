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
        private const int MaxRetryAttempts = 3; // Maximum number of retry attempts
        private const int DelayBetweenRetries = 1000; // Delay between retries in milliseconds

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
                        // Check if the file is older than 1 minute
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

                await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken); // Check every 4 minutes
            }
        }

        private void UnlockAndDeleteFile(string filePath)
        {
            for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                try
                {
                    // If the file is not locked, delete it
                    File.Delete(filePath);
                    Debug.WriteLine($"Successfully deleted file: {filePath}");
                    return; // Exit if deletion was successful
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"Attempt {attempt} of {MaxRetryAttempts} failed to delete file {filePath}: {ex.Message}.");

                    // Wait before retrying
                    if (attempt < MaxRetryAttempts)
                    {
                        Thread.Sleep(DelayBetweenRetries);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unexpected error while trying to delete file: {filePath}, error: {ex.Message}");
                    return; // Exit on unexpected errors
                }
            }

            Debug.WriteLine($"Skipping deletion of file {filePath} after {MaxRetryAttempts} attempts.");
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
