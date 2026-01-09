using HomeFlow.Data;
using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Services;

namespace HomeFlow.Features.MealPlanning.GroceryStores;

public record GetGroceryStoreTableDataQuery( QueryOptions QueryOptions ) : IRequest<TableData<GroceryStore>>;

public class GetGroceryStoreTableDataQueryHandler : IRequestHandler<GetGroceryStoreTableDataQuery, TableData<GroceryStore>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetGroceryStoreTableDataQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TableData<GroceryStore>> Handle( GetGroceryStoreTableDataQuery request, CancellationToken cancellationToken )
    {
        var query = _context.GroceryStores.AsQueryable();

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
            case "locations":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.Location )
                    : query.OrderBy( r => r.Location );
                break;
            case "aisles":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( r => r.GroceryStoreAisles.Count )
                    : query.OrderBy( r => r.GroceryStoreAisles.Count );
                break;
            default:
                query = query.OrderBy( r => r.Name );
                break;
        }

        var total = query.Count();

        if ( request.QueryOptions.PageSize > 0 )
        {
            query = query.Skip( request.QueryOptions.Page * request.QueryOptions.PageSize )
                            .Take( request.QueryOptions.PageSize );
        }

        var groceryStoreEntities = await query
            .Include( gs => gs.GroceryStoreAisles )
                .ThenInclude( a => a.GroceryStoreAisleGroceryItems )
                .ThenInclude( ai => ai.GroceryItem )
            .ToListAsync();

        var groceryStores = new List<GroceryStore>();

        foreach ( var entity in groceryStoreEntities )
        {
            GroceryStore groceryStore = new GroceryStore
            {
                Id = entity.Id,
                Name = entity.Name,
                Location = entity.Location
            };

            groceryStores.Add( groceryStore );

            foreach ( var item in entity.GroceryStoreAisles.OrderBy( a => a.Order ) )
            {
                groceryStore.GroceryStoreAisles.Add( new GroceryStoreAisle
                {
                    Id = item.Id,
                    Name = item.Name,
                    Order = item.Order,
                    GroceryItems = item.GroceryStoreAisleGroceryItems
                        .Select( i => new GroceryItem
                        {
                            Id = i.GroceryItem.Id,
                            Name = i.GroceryItem.Name,
                        } )
                        .ToList()
                } );
            }
        }

        return new TableData<GroceryStore>
        {
            TotalItems = total,
            Items = groceryStores
        };
    }
}
