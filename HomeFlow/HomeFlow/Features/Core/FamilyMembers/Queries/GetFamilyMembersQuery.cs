using HomeFlow.Data;

namespace HomeFlow.Features.Core.FamilyMembers;

public record GetFamilyMembersQuery : IRequest<List<FamilyMember>>;

public class GetFamilyMembersQueryHandler : IRequestHandler<GetFamilyMembersQuery, List<FamilyMember>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetFamilyMembersQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<FamilyMember>> Handle( GetFamilyMembersQuery request, CancellationToken cancellationToken )
    {
        var familyMembers = await _context.FamilyMembers
            .Include( x => x.Image )
            .ProjectTo<FamilyMember>( _mapper.ConfigurationProvider )
            .OrderBy( x => x.FamilyMemberType )
            .ToListAsync( cancellationToken );

        return familyMembers;
    }
}
