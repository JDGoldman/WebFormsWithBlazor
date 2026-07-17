using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorAppWithFormsAuthentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAppWithFormsAuthentication.Controllers
{
    [ApiController]
    [Route("api/rtv/{rtvId:int}/files/{fileId}")]
    public class RtvFileController : ControllerBase
    {
        private readonly IRtvLinkedFileService _linkedFileService;

        public RtvFileController(IRtvLinkedFileService linkedFileService)
        {
            _linkedFileService = linkedFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Download(int rtvId, string fileId)
        {
            var files = await _linkedFileService.GetFilesAsync(rtvId);
            var linkedFile = files.FirstOrDefault(file => file.Id == fileId);
            if (linkedFile == null)
            {
                return NotFound();
            }

            var storedFilePath = _linkedFileService.GetStoredFilePath(rtvId, linkedFile.StoredFileName);
            if (!System.IO.File.Exists(storedFilePath))
            {
                return NotFound();
            }

            var contentType = "application/octet-stream";
            return PhysicalFile(storedFilePath, contentType, linkedFile.OriginalFileName);
        }
    }
}
