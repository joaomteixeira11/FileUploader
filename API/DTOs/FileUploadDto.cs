// *******************************************************************************
//   This code defines a Data Transfer Object (DTO) for file uploads in an API. 
//  It includes a single property for the file being uploaded, which is required.
// *******************************************************************************

namespace API.DTOs
{
    public class FileUploadDto
    {
        public required IFormFile File { get; set; }
    }
}

