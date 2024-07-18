namespace API.Services
{
    public interface IKasperskyService
    {
        Task<string?> ScanFileAsync(IFormFile file);
    }
}
