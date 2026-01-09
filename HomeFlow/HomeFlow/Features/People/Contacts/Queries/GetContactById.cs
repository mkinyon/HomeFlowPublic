using HomeFlow.Data;

namespace HomeFlow.Features.People.Contacts;

public record GetContactById( Guid Id ) : IRequest<Contact?>;

public class GetContactByIdHandler : IRequestHandler<GetContactById, Contact?>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetContactByIdHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Contact?> Handle( GetContactById request, CancellationToken cancellationToken )
    {
        var contact = await _context.Contacts
            .Include( x => x.Image )
            .ProjectTo<Contact>( _mapper.ConfigurationProvider )
            .FirstOrDefaultAsync( x => x.Id == request.Id, cancellationToken );

        return contact;
    }
}
