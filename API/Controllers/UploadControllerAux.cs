using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using API.Entities;
using API.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadControllerAux(IWebHostEnvironment env, ILogger<UploadController> logger, IHttpClientFactory clientFactory, DataContext context) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly ILogger<UploadController> _logger = logger;
        private readonly IHttpClientFactory _clientFactory = clientFactory;
        private readonly DataContext _context = context;

        [HttpPost]
        public async Task<ActionResult> UploadFile([FromForm] FileUploadDto fileUploadDto)
        {
            _logger.LogInformation("UploadFile endpoint hit");

            if (fileUploadDto.File == null || fileUploadDto.File.Length == 0)
            {
                _logger.LogWarning("No file uploaded.");
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
                    _logger.LogInformation("Saving file to disk.");
                    await fileUploadDto.File.CopyToAsync(stream);
                }

                _logger.LogInformation("File uploaded successfully.");

                // Add to the DB
                var files = new Files
                {
                    FileName = fileUploadDto.File.FileName,
                    FilePath = filePath,
                    UploadedAt = DateTime.UtcNow
                };
                _context.Files.Add(files);
                await _context.SaveChangesAsync();

                // Redirecionar o arquivo para a plataforma de destino
                var httpClient = _clientFactory.CreateClient("custom");
                var requestContent = new MultipartFormDataContent();

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = fileUploadDto.File.FileName
                    };
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(fileUploadDto.File.ContentType);

                    requestContent.Add(streamContent);

                    var response = await httpClient.PostAsync("https://10.222.56.38:1234/upload", requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("File redirected successfully.");
                        return Ok(new { Message = "File uploaded and redirected successfully!", Response = result });
                    }

                    _logger.LogError($"Failed to redirect file. Status Code: {response.StatusCode}, Response: {await response.Content.ReadAsStringAsync()}");
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
                return StatusCode(500, "Internal server error");
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
                    UploadedAt = file.UploadedAt.ToString("yyyy-MM-dd HH:mm:ss") // Formatando a data e hora
                }).ToListAsync();

            return Ok(files);
        }
    }
}
