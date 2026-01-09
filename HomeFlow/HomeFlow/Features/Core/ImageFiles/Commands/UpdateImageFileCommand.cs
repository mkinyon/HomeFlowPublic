using HomeFlow.Data;
using HomeFlow.Infrastructure.FileStorage;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace HomeFlow.Features.Core.ImageFiles;

public record UpdateImageFileCommand( Guid Id, ImageFileRequest Request ) : IRequest<ImageFile>;

public class UpdateImageFileCommandHandler : IRequestHandler<UpdateImageFileCommand, ImageFile>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IFileStorage _fileStorage;

    public UpdateImageFileCommandHandler( IHomeFlowDbContext context, IFileStorage fileStorage )
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<ImageFile> Handle( UpdateImageFileCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.ImageFiles.FirstOrDefaultAsync( i => i.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, entity );

        if ( request.Request.Width <= 0 || request.Request.Height <= 0 )
            throw new ArgumentException( "Invalid image dimensions." );

        using var image = Image.Load( request.Request.Data );

        var resizeOptions = new ResizeOptions
        {
            Mode = ResizeMode.Crop,
            Size = new Size( request.Request.Width, request.Request.Height )
        };

        image.Mutate( x => x.Resize( resizeOptions ) );

        using var stream = new MemoryStream();
        image.Save( stream, new PngEncoder() );

        stream.Position = 0;

        // Update the existing file in storage
        await _fileStorage.UpdateAsync( stream, entity.Path, cancellationToken );

        // Update the entity if folder has changed
        if ( !string.IsNullOrEmpty( request.Request.Folder ) && entity.Folder != request.Request.Folder )
        {
            entity.Folder = request.Request.Folder;
        }

        await _context.SaveChangesAsync( cancellationToken );

        return new ImageFile
        {
            Id = entity.Id,
            MimeType = entity.MimeType,
            Url = entity.Url,
            Folder = entity.Folder
        };
    }
}
