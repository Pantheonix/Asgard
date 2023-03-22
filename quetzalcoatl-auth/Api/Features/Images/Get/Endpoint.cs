namespace Api.Features.Images.Get;

public class GetImageEndpoint : Endpoint<GetImageRequest>
{
    private readonly IPictureRepository _pictureRepository;
    private readonly ILogger<GetImageEndpoint> _logger;

    public GetImageEndpoint(IPictureRepository pictureRepository, ILogger<GetImageEndpoint> logger)
    {
        _pictureRepository =
            pictureRepository ?? throw new ArgumentNullException(nameof(pictureRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Get("{id}");
        Group<ImagesGroup>();
    }

    public override async Task HandleAsync(GetImageRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Getting image with id {Id}", req.Id.ToString());

        var image = await _pictureRepository.GetPictureByIdAsync(req.Id, ct);

        if (image is null)
        {
            _logger.LogWarning("Image with id {Id} not found", req.Id.ToString());
            await SendNotFoundAsync(ct);
            return;
        }

        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.ContentType = "image/jpeg";
        await HttpContext.Response.Body.WriteAsync(image.Data, ct);
    }
}
