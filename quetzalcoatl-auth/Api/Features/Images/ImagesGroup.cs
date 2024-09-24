namespace Api.Features.Images;

public sealed class ImagesGroup : Group
{
    public ImagesGroup()
    {
        Configure(
            "images",
            ep =>
            {
                ep.Description(builder => builder.AllowAnonymous());
            }
        );
    }
}
