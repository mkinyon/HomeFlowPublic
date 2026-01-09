using HomeFlow.Data;
using HomeFlow.Infrastructure.FileStorage;
using MediatR;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace HomeFlow.Features.Core.ImageFiles;

public record UploadImageFileCommand( ImageFileRequest Request ) : IRequest<ImageFile>;

public class UploadImageFileCommandHandler : IRequestHandler<UploadImageFileCommand, ImageFile>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IFileStorage _fileStorage;

    public UploadImageFileCommandHandler( IHomeFlowDbContext context, IFileStorage fileStorage )
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<ImageFile> Handle( UploadImageFileCommand request, CancellationToken cancellationToken )
    {
        var imageGuid = Guid.NewGuid();
        var fileName = $"{imageGuid}.png";

        using var image = Image.Load( request.Request.Data );

        if ( request.Request.Width <= 0 || request.Request.Height <= 0 )
            throw new ArgumentException( "Invalid image dimensions." );

        var resizeOptions = new ResizeOptions
        {
            Mode = ResizeMode.Crop,
            Size = new Size( request.Request.Width, request.Request.Height )
        };

        image.Mutate( x => x.Resize( resizeOptions ) );

        using var stream = new MemoryStream();
        image.Save( stream, new PngEncoder() );

        stream.Position = 0;
        var path = await _fileStorage.UploadAsync( stream, fileName, request.Request.Folder, cancellationToken );

        var entity = new ImageFileEntity
        {
            Id = imageGuid,
            Path = path,
            MimeType = "image/png",
            Url = path.Replace( "\\", "/" ),
            Folder = request.Request.Folder
        };

        _context.ImageFiles.Add( entity );

        await _context.SaveChangesAsync( cancellationToken );

        return new ImageFile
        {
            Id = entity.Id,
            Url = entity.Url,
            MimeType = entity.MimeType,
            Folder = entity.Folder
        };
    }
}