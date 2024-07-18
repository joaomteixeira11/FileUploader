using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController(IKasperskyService kasperskyService, ILogger<UploadController> logger) : ControllerBase
    {
        private readonly IKasperskyService _kasperskyService = kasperskyService ?? throw new ArgumentNullException(nameof(kasperskyService));
        private readonly ILogger<UploadController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto fileUploadDto)
        {
            _logger.LogInformation("UploadFile endpoint hit");

            if (fileUploadDto.File == null || fileUploadDto.File.Length == 0)
            {
                _logger.LogWarning("No file uploaded.");
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Trying to scan file with Kaspersky
                var scanResult = await _kasperskyService.ScanFileAsync(fileUploadDto.File);
                _logger.LogInformation($"Scan result: {scanResult}");

                return Ok(new { message = "File uploaded successfully!", fileName = fileUploadDto.File.FileName, scanResult });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error scanning file with Kaspersky: {ex.Message}");
                return Ok(new { message = "Failed", fileName = fileUploadDto.File.FileName, scanResult = "Failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred while uploading the file.");
            }
        }
    }
}
