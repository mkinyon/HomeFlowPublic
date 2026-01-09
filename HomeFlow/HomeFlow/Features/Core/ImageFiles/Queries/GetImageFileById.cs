using HomeFlow.Data;

namespace HomeFlow.Features.Core.ImageFiles;

public record GetImageFileById( Guid Id ) : IRequest<ImageFileEntity?>;

public class GetImageFileByIdHandler : IRequestHandler<GetImageFileById, ImageFileEntity?>
{
    private readonly IHomeFlowDbContext _context;

    public GetImageFileByIdHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<ImageFileEntity?> Handle( GetImageFileById request, CancellationToken cancellationToken )
    {
        var entity = await _context.ImageFiles.FirstOrDefaultAsync( i => i.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, entity );

        ImageFileEntity imageFile = new ImageFileEntity
        {
            Id = entity.Id,
            Url = entity.Url
        };

        return imageFile;
    }
}
