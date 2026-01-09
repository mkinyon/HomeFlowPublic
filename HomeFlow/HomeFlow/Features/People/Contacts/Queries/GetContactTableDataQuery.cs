using HomeFlow.Data;
using HomeFlow.Services;

namespace HomeFlow.Features.People.Contacts;

public record GetContactTableDataQuery( QueryOptions Options ) : IRequest<TableData<Contact>>;

public class GetContactTableDataQueryHandler : IRequestHandler<GetContactTableDataQuery, TableData<Contact>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetContactTableDataQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TableData<Contact>> Handle( GetContactTableDataQuery request, CancellationToken cancellationToken )
    {
        var query = _context.Contacts
            .Include( x => x.Image )
            .AsQueryable();

        // Apply search filter
        if ( !string.IsNullOrWhiteSpace( request.Options.SearchTerm ) )
        {
            var searchTerm = request.Options.SearchTerm.ToLower();
            query = query.Where( x =>
                x.FirstName.ToLower().Contains( searchTerm ) ||
                x.LastName.ToLower().Contains( searchTerm ) ||
                x.Email.ToLower().Contains( searchTerm ) ||
                x.PhoneNumber.Contains( searchTerm )
            );
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync( cancellationToken );

        // sorting
        switch ( request.Options.SortBy )
        {
            case "firstname":
                query = request.Options.SortDescending
                    ? query.OrderByDescending( x => x.FirstName )
                    : query.OrderBy( x => x.FirstName );
                break;
            case "lastname":
                query = request.Options.SortDescending
                    ? query.OrderByDescending( x => x.LastName )
                    : query.OrderBy( x => x.LastName );
                break;
            case "email":
                query = request.Options.SortDescending
                    ? query.OrderByDescending( x => x.Email )
                    : query.OrderBy( x => x.Email );
                break;
            case "birthdate":
                query = request.Options.SortDescending
                    ? query.OrderByDescending( x => x.BirthDate )
                    : query.OrderBy( x => x.BirthDate );
                break;
            default:
                query = query.OrderBy( x => x.LastName ).ThenBy( x => x.FirstName );
                break;
        }

        // Apply pagination
        if ( request.Options.PageSize > 0 )
        {
            query = query.Skip( request.Options.Page * request.Options.PageSize )
                        .Take( request.Options.PageSize );
        }

        var contacts = await query
            .ProjectTo<Contact>( _mapper.ConfigurationProvider )
            .ToListAsync( cancellationToken );

        return new TableData<Contact>
        {
            TotalItems = totalCount,
            Items = contacts
        };
    }
}
