namespace HomeFlow.Features.Core.ImageFiles;

public class ImageFile
{
    public Guid Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public string Folder { get; set; } = string.Empty;
}

