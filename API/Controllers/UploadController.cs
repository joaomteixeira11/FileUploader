using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Entities;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController(IWebHostEnvironment env, ILogger<UploadController> logger, DataContext context, KasperskyService kasperskyService) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = env ?? throw new ArgumentNullException(nameof(env));
        private readonly ILogger<UploadController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly DataContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly KasperskyService _kasperskyService = kasperskyService ?? throw new ArgumentNullException(nameof(kasperskyService));

        [HttpPost]
        public async Task<ActionResult> UploadFile([FromForm] FileUploadDto fileUploadDto)
        {
            _logger.LogInformation("UploadFile endpoint hit");

            if (fileUploadDto.File == null || fileUploadDto.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadPath = Path.Combine(_env.ContentRootPath, "Uploads");

            if (!Directory.Exists(uploadPath))
            {
                _logger.LogInformation("Creating upload directory.");
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileUploadDto.File.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _logger.LogInformation("Saving file to disk...");
                    await fileUploadDto.File.CopyToAsync(stream);
                    _logger.LogInformation("Saved!");
                }

                _logger.LogInformation("File uploaded successfully to the disk.");

                // Add to the DB
                var files = new Files
                {
                    FileName = fileUploadDto.File.FileName,
                    FilePath = filePath,
                    UploadedAt = DateTime.UtcNow
                };
                _context.Files.Add(files);
                await _context.SaveChangesAsync();

                // Scan the file with Kaspersky
                try
                {
                    _logger.LogInformation($"Trying to scan file ({filePath}) with Kaspersky...");
                    var scanResult = await _kasperskyService.ScanFileAsync(filePath);
                    _logger.LogInformation("Scaned!!! Uploading to kaspersky...");
                    return Ok(new { message = "File uploaded successfully!", fileName = fileUploadDto.File.FileName, scanResult });
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error scanning file with Kaspersky.");
                    return Ok(new { message = "Failed", fileName = fileUploadDto.File.FileName, scanResult = "Failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
                return StatusCode(500, "An error occurred while uploading the file.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Files>>> GetFiles()
        {
            var files = await _context.Files
                .Select(file => new FilesDto
                {
                    FileName = file.FileName,
                    FilePath = file.FilePath,
                    UploadedAt = file.UploadedAt.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToListAsync();

            return Ok(files);
        }
    }
}
