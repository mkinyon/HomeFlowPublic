using HomeFlow.Models;

namespace HomeFlow.Features.Core.ImageFiles;

public class ImageFileEntity : Model
{
    public string Path { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Folder { get; set; } = string.Empty;
}