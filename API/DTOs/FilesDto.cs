namespace API.DTOs;

public class FilesDto
{
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required string UploadedAt { get; set; } // String para armazenar a data e hora formatada
}
