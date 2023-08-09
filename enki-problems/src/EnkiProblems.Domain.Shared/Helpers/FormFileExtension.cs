using System.IO;
using Microsoft.AspNetCore.Http;

namespace EnkiProblems.Helpers;

public static class FormFileExtension
{
    public static byte[] GetBytes(this IFormFile formFile)
    {
        using var memoryStream = new MemoryStream();
        formFile.OpenReadStream().CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}
