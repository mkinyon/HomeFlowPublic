namespace HomeFlow.Features.Core.ImageFiles;

public class ImageFileRequest
{
    public int Width { get; set; }

    public int Height { get; set; }

    public byte[] Data { get; set; } = Array.Empty<byte>();

    public string Folder { get; set; } = string.Empty;
}
