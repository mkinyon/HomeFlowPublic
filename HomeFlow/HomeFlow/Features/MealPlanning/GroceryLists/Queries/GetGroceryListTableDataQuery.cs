using HomeFlow.Data;
using HomeFlow.Services;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record GetGroceryStoreTableDataQuery( QueryOptions QueryOptions ) : IRequest<TableData<GroceryList>>;

public class GetGroceryListTableDataQueryHanderer : IRequestHandler<GetGroceryStoreTableDataQuery, TableData<GroceryList>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetGroceryListTableDataQueryHanderer( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TableData<GroceryList>> Handle( GetGroceryStoreTableDataQuery request, CancellationToken cancellationToken )
    {
        var query = _context.GroceryLists.AsQueryable();

        // search
        string searchString = (request.QueryOptions.SearchTerm ?? string.Empty).ToLower();
        if ( searchString != string.Empty )
        {
            query = query.Where( r => r.Name.ToLower().Contains( searchString ) );
        }

        // sorting
        switch ( request.QueryOptions.SortBy )
        {
            case "name":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.Name )
                    : query.OrderBy( r => r.Name );
                break;
            case "items":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.Items.Count )
                    : query.OrderBy( r => r.Items.Count );
                break;
            case "created":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.Created )
                    : query.OrderBy( r => r.Created );
                break;
            case "completed":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.IsPrinted )
                    : query.OrderBy( r => r.IsPrinted );
                break;
            default:
                query = query.OrderByDescending( r => r.Created );
                break;
        }

        // additional settings
        bool includeCompleted = false;
        bool.TryParse( request.QueryOptions.AdditionalSettings.GetValueOrDefault( "IncludeCompleted" ) ?? "", out includeCompleted );
        if ( !includeCompleted )
        {
            query = query.Where( i => !i.IsPrinted );
        }

        var total = query.Count();

        if ( request.QueryOptions.PageSize > 0 )
        {
            query = query.Skip( request.QueryOptions.Page * request.QueryOptions.PageSize )
                            .Take( request.QueryOptions.PageSize );
        }

        var groceryLists = await query
            .Include( r => r.Items )
            .ProjectTo<GroceryList>( _mapper.ConfigurationProvider )
            .ToListAsync( cancellationToken );

        return new TableData<GroceryList>
        {
            TotalItems = total,
            Items = groceryLists
        };
    }
}
