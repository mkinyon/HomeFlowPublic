using HomeFlow.Data;

namespace HomeFlow.Features.People.Contacts;

public record GetContactsQuery : IRequest<List<Contact>>;

public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, List<Contact>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetContactsQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Contact>> Handle( GetContactsQuery request, CancellationToken cancellationToken )
    {
        var contacts = await _context.Contacts
            .Include( x => x.Image )
            .ProjectTo<Contact>( _mapper.ConfigurationProvider )
            .OrderBy( x => x.LastName )
            .ThenBy( x => x.FirstName )
            .ToListAsync( cancellationToken );

        return contacts;
    }
}
