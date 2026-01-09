using HomeFlow.Data;
using HomeFlow.Infrastructure.FileStorage;

namespace HomeFlow.Features.Core.FamilyMembers;

public record CreateFamilyMemberCommand( FamilyMember FamilyMember ) : IRequest<Guid>;

public class CreateFamilyMemberCommandHandler : IRequestHandler<CreateFamilyMemberCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IFileStorage _fileStorage;

    public CreateFamilyMemberCommandHandler( IHomeFlowDbContext context, IFileStorage fileStorage )
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Guid> Handle( CreateFamilyMemberCommand request, CancellationToken cancellationToken )
    {
        var entity = new FamilyMemberEntity
        {
            FirstName = request.FamilyMember.FirstName,
            LastName = request.FamilyMember.LastName,
            FamilyMemberType = request.FamilyMember.FamilyMemberType,
            Gender = request.FamilyMember.Gender
        };

        if ( request.FamilyMember.Image != null )
        {
            var imageEntity = _context.ImageFiles.FirstOrDefault( i => i.Id == request.FamilyMember.Image.Id );
            entity.Image = imageEntity;
        }

        _context.FamilyMembers.Add( entity );

        await _context.SaveChangesAsync( cancellationToken );

        return entity.Id;
    }
}