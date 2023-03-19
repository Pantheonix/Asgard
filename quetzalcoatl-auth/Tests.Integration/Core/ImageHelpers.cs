namespace Tests.Integration.Core;

public static class ImageHelpers
{
    private static async Task<byte[]> GetImageAsByteArrayAsync(string urlImage)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(urlImage);

        return await response.Content.ReadAsByteArrayAsync();
    }

    public static async Task<IFormFile> GetImageAsFormFileAsync(string urlImage, string fileName)
    {
        var image = await GetImageAsByteArrayAsync(urlImage);

        var stream = new MemoryStream(image);
        var file = new FormFile(stream, 0, image.Length, null!, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpg"
        };
        return file;
    }
}
