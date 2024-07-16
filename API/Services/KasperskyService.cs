using System.Text;
using System.Text.Json;

namespace API.Services
{
    public class KasperskyService(IHttpClientFactory httpClientFactory, ILogger<KasperskyService> logger)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger<KasperskyService> _logger = logger;

        public async Task<string> ScanFileAsync(string filePath)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("KasperskyClient");

                if (!Path.IsPathRooted(filePath))
                {
                    throw new ArgumentException("The file path must be absolute.");
                }

                _logger.LogInformation($"Absolute file path: {filePath}");

                // Ler o conteúdo do arquivo e codificá-lo em Base64
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var base64FileContent = Convert.ToBase64String(fileBytes);

                // Criar o conteúdo da solicitação no formato JSON esperado
                var scanRequest = new
                {
                    timeout = "10000",
                    @object = base64FileContent // Conteúdo do arquivo codificado em Base64
                };

                var json = JsonSerializer.Serialize(scanRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending request to Kaspersky Scan Engine...");
                var response = await client.PostAsync("/api/v3.0/scanmemory", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error uploading file to Kaspersky Scan Engine: {response.StatusCode}, Content: {errorContent}");

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new HttpRequestException($"Bad Request: {errorContent}");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new HttpRequestException($"Internal Server Error: {errorContent}");
                    }
                    else
                    {
                        throw new HttpRequestException($"Error uploading file to Kaspersky Scan Engine: {response.StatusCode}, Content: {errorContent}");
                    }
                }

                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from Kaspersky Scan Engine!");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while scanning the file.");
                throw;
            }
        }
    }
}
