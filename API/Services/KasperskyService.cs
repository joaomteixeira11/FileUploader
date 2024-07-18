using System.Text.Json;
using System.Text;

namespace API.Services
{
    public class KasperskyService(IHttpClientFactory httpClientFactory, ILogger<KasperskyService> logger) : IKasperskyService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger<KasperskyService> _logger = logger;

        public async Task<string?> ScanFileAsync(IFormFile file)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("KasperskyClient");

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    var base64FileContent = Convert.ToBase64String(fileBytes);

                    var scanRequest = new
                    {
                        timeout = "10000",
                        @object = base64FileContent
                    };

                    var json = JsonSerializer.Serialize(scanRequest);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    _logger.LogInformation("Sending request to Kaspersky Scan Engine...");
                    var response = await client.PostAsync("/api/v3.0/scanmemory", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"Error uploading file to Kaspersky Scan Engine: {response.StatusCode}, Content: {errorContent}");
                        throw new HttpRequestException($"Error uploading file to Kaspersky Scan Engine: {response.StatusCode}, Content: {errorContent}");
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received response from Kaspersky Scan Engine!");

                    using (JsonDocument doc = JsonDocument.Parse(result))
                    {
                        if (doc.RootElement.TryGetProperty("scanResult", out var scanResultProperty))
                        {
                            return scanResultProperty.GetString();
                        }
                        else
                        {
                            _logger.LogError("scanResult property not found in response");
                            return "Unknown";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while scanning the file.");
                throw;
            }
        }
    }
}
