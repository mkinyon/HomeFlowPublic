using HomeFlow.Data;

namespace HomeFlow.Features.Core.FamilyMembers;

public record DeleteFamilyMemberCommand( Guid Id ) : IRequest;

public class DeleteFamilyMemberCommandHandler : IRequestHandler<DeleteFamilyMemberCommand>
{
    private readonly IHomeFlowDbContext _context;

    public DeleteFamilyMemberCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( DeleteFamilyMemberCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.FamilyMembers.FirstOrDefaultAsync( i => i.Id == request.Id, cancellationToken );

        Guard.Against.NotFound( request.Id, entity );

        _context.FamilyMembers.Remove( entity );
        await _context.SaveChangesAsync( cancellationToken );
    }
}