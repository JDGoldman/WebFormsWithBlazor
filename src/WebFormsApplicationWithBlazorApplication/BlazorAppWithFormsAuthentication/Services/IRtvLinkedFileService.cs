using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorAppWithFormsAuthentication.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorAppWithFormsAuthentication.Services
{
    public interface IRtvLinkedFileService
    {
        Task<IReadOnlyList<RtvLinkedFile>> GetFilesAsync(int rtvId);
        Task<RtvLinkedFile> SaveFileAsync(int rtvId, IBrowserFile file);
        Task<bool> DeleteFileAsync(int rtvId, string fileId);
        string GetStoredFilePath(int rtvId, string storedFileName);
    }
}
