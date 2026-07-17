using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorAppWithFormsAuthentication.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;

namespace BlazorAppWithFormsAuthentication.Services
{
    public class RtvLinkedFileService : IRtvLinkedFileService
    {
        public const long MaxFileSizeBytes = 10 * 1024 * 1024;

        private readonly IWebHostEnvironment _environment;

        public RtvLinkedFileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<IReadOnlyList<RtvLinkedFile>> GetFilesAsync(int rtvId)
        {
            var manifestPath = GetManifestPath(rtvId);
            if (!File.Exists(manifestPath))
            {
                return Array.Empty<RtvLinkedFile>();
            }

            await using var stream = File.OpenRead(manifestPath);
            var files = await JsonSerializer.DeserializeAsync<List<RtvLinkedFile>>(stream) ?? new List<RtvLinkedFile>();
            return files.OrderByDescending(file => file.UploadedAtUtc).ToList();
        }

        public async Task<RtvLinkedFile> SaveFileAsync(int rtvId, IBrowserFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.Size > MaxFileSizeBytes)
            {
                throw new InvalidOperationException($"File '{file.Name}' exceeds the maximum size of {MaxFileSizeBytes / (1024 * 1024)} MB.");
            }

            var directory = GetRtvDirectory(rtvId);
            Directory.CreateDirectory(directory);

            var safeOriginalName = Path.GetFileName(file.Name);
            var storedFileName = $"{Guid.NewGuid():N}_{safeOriginalName}";
            var storedFilePath = Path.Combine(directory, storedFileName);

            await using (var readStream = file.OpenReadStream(MaxFileSizeBytes))
            await using (var writeStream = File.Create(storedFilePath))
            {
                await readStream.CopyToAsync(writeStream);
            }

            var linkedFile = new RtvLinkedFile
            {
                Id = Guid.NewGuid().ToString("N"),
                OriginalFileName = safeOriginalName,
                StoredFileName = storedFileName,
                SizeBytes = file.Size,
                UploadedAtUtc = DateTime.UtcNow
            };

            var files = (await GetFilesAsync(rtvId)).ToList();
            files.Add(linkedFile);
            await WriteManifestAsync(rtvId, files);

            return linkedFile;
        }

        public async Task<bool> DeleteFileAsync(int rtvId, string fileId)
        {
            var files = (await GetFilesAsync(rtvId)).ToList();
            var linkedFile = files.FirstOrDefault(file => file.Id == fileId);
            if (linkedFile == null)
            {
                return false;
            }

            var storedFilePath = GetStoredFilePath(rtvId, linkedFile.StoredFileName);
            if (File.Exists(storedFilePath))
            {
                File.Delete(storedFilePath);
            }

            files.Remove(linkedFile);
            await WriteManifestAsync(rtvId, files);
            return true;
        }

        public string GetStoredFilePath(int rtvId, string storedFileName)
        {
            return Path.Combine(GetRtvDirectory(rtvId), storedFileName);
        }

        private string GetRtvDirectory(int rtvId)
        {
            return Path.Combine(_environment.ContentRootPath, "App_Data", "RtvFiles", rtvId.ToString());
        }

        private string GetManifestPath(int rtvId)
        {
            return Path.Combine(GetRtvDirectory(rtvId), "manifest.json");
        }

        private async Task WriteManifestAsync(int rtvId, IReadOnlyList<RtvLinkedFile> files)
        {
            var manifestPath = GetManifestPath(rtvId);
            Directory.CreateDirectory(Path.GetDirectoryName(manifestPath));

            await using var stream = File.Create(manifestPath);
            await JsonSerializer.SerializeAsync(stream, files, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}
