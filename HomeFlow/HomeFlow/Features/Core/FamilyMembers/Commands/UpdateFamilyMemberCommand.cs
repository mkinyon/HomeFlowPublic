using HomeFlow.Data;
using HomeFlow.Infrastructure.FileStorage;

namespace HomeFlow.Features.Core.FamilyMembers;

public record UpdateFamilyMemberCommand( FamilyMember FamilyMember ) : IRequest;

public class UpdateFamilyMemberCommandHandler : IRequestHandler<UpdateFamilyMemberCommand>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IFileStorage _fileStorage;

    public UpdateFamilyMemberCommandHandler( IHomeFlowDbContext context, IFileStorage fileStorage )
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task Handle( UpdateFamilyMemberCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.FamilyMembers.FirstOrDefaultAsync( i => i.Id == request.FamilyMember.Id, cancellationToken );

        Guard.Against.NotFound( request.FamilyMember.Id, entity );

        entity.FirstName = request.FamilyMember.FirstName;
        entity.LastName = request.FamilyMember.LastName;
        entity.FamilyMemberType = request.FamilyMember.FamilyMemberType;
        entity.Gender = request.FamilyMember.Gender;

        if ( request.FamilyMember.Image != null )
        {
            var imageEntity = _context.ImageFiles.FirstOrDefault( i => i.Id == request.FamilyMember.Image.Id );
            entity.Image = imageEntity;
        }
        else
        {
            entity.Image = null;
            entity.ImageId = null;
        }

        await _context.SaveChangesAsync( cancellationToken );
    }
}