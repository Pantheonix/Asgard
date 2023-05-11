namespace Api.Features.Core;

public static class FormFileExtensions
{
    public static byte[] GetBytes(this IFormFile formFile)
    {
        using var memoryStream = new MemoryStream();
        formFile.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}
