namespace Api.Features.Images.Get;

public class GetImageEndpoint : Endpoint<GetImageRequest>
{
    private readonly AppDbContext _context;

    public GetImageEndpoint(AppDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("{id}");
        Group<ImagesGroup>();
    }

    public override async Task HandleAsync(GetImageRequest req, CancellationToken ct)
    {
        var image = await _context.Pictures.FirstOrDefaultAsync(
            p => p.Id == req.Id,
            cancellationToken: ct
        );

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
