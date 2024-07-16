using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Entities;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.IO;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UploadController> _logger;
        private readonly DataContext _context;
        private readonly KasperskyService _kasperskyService;

        public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger, DataContext context, KasperskyService kasperskyService)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _kasperskyService = kasperskyService ?? throw new ArgumentNullException(nameof(kasperskyService));
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile([FromForm] FileUploadDto fileUploadDto)
        {
            _logger.LogInformation("(UploadController)UploadFile endpoint hit");

            if (fileUploadDto.File == null || fileUploadDto.File.Length == 0)
            {
                _logger.LogWarning("(UploadController)No file uploaded.");
                return BadRequest("No file uploaded.");
            }

            var uploadPath = Path.Combine(_env.ContentRootPath, "Uploads");

            if (!Directory.Exists(uploadPath))
            {
                _logger.LogInformation("(UploadController)Creating upload directory.");
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileUploadDto.File.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _logger.LogInformation("(UploadController)Saving file to disk.");
                    await fileUploadDto.File.CopyToAsync(stream);
                }

                _logger.LogInformation("(UploadController)File uploaded successfully to the disk.");

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
                    _logger.LogInformation($"(UploadController) Trying to scan file ({filePath}) with Kaspersky...");
                    var scanResult = await _kasperskyService.ScanFileAsync(filePath);
                    _logger.LogInformation("(UploadController)Trying to upload dos kaspersky...");
                    return Ok(new { message = "File uploaded successfully!", fileName = fileUploadDto.File.FileName, scanResult });
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "(UploadController)Error scanning file with Kaspersky.");
                    return Ok(new { message = "Failed", fileName = fileUploadDto.File.FileName, scanResult = "Failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "(UploadController)An error occurred while uploading the file.");
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
