

    using Renci.SshNet;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SftpService
    {
        private readonly string host = "192.168.243.151";
        private readonly string username = "tester";
        private readonly string password = "password";

    public void DownloadFiles(string remoteDirectory, string localDirectory)
        {
            using (var sftp = new SftpClient(host, username, password))
            {
                sftp.Connect();

                // Change to the remote directory
                sftp.ChangeDirectory(remoteDirectory);

                // List directory contents
                var files = sftp.ListDirectory(remoteDirectory);
                foreach (var file in files)
                {
                    Console.WriteLine(file.FullName);
                }

                // Filter and download files
                var validFiles = files.Where(f => !f.IsDirectory && IsValidFile(f.Name)).ToList();
                foreach (var file in validFiles)
                {
                    string remoteFilePath = $"{remoteDirectory}/{file.Name}";
                    string localFilePath = Path.Combine(localDirectory, file.Name);

                    using (var fileStream = File.Create(localFilePath))
                    {
                        sftp.DownloadFile(remoteFilePath, fileStream);
                    }
                }
                sftp.Disconnect();
            }
        }

        private bool IsValidFile(string fileName)
        {
            var validExtensions = new[] { ".csv", ".txt" };
            var fileExtension = Path.GetExtension(fileName).ToLower();
            return validExtensions.Contains(fileExtension);
        }
    }

