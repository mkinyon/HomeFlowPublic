using HomeFlow.Data;

namespace HomeFlow.Features.Core.FamilyMembers;

public record GetFamilyMemberById( Guid Id ) : IRequest<FamilyMember?>;

public class GetFamilyMemberByIdHandler : IRequestHandler<GetFamilyMemberById, FamilyMember?>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetFamilyMemberByIdHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FamilyMember?> Handle( GetFamilyMemberById request, CancellationToken cancellationToken )
    {
        var familyMember = await _context.FamilyMembers
            .Include( x => x.Image )
            .ProjectTo<FamilyMember>( _mapper.ConfigurationProvider )
            .FirstOrDefaultAsync( x => x.Id == request.Id, cancellationToken );

        return familyMember;
    }
}
