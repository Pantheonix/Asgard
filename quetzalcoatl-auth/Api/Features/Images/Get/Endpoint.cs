namespace Api.Features.Images.Get;

public class GetImageEndpoint : Endpoint<GetImageRequest>
{
    private readonly IPictureRepository _pictureRepository;

    public GetImageEndpoint(IPictureRepository pictureRepository)
    {
        _pictureRepository =
            pictureRepository ?? throw new ArgumentNullException(nameof(pictureRepository));
    }

    public override void Configure()
    {
        Get("{id}");
        Group<ImagesGroup>();
    }

    public override async Task HandleAsync(GetImageRequest req, CancellationToken ct)
    {
        var image = await _pictureRepository.GetPictureByIdAsync(req.Id, ct);

        if (image is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.ContentType = "image/jpeg";
        await HttpContext.Response.Body.WriteAsync(image.Data, ct);
    }
}
