using HomeFlow.Data;
using HomeFlow.Services;

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public record GetGroceryItemTableDataQuery( QueryOptions QueryOptions ) : IRequest<TableData<GroceryItemVM>>;

public class GetGroceryItemTableDataQueryHandler : IRequestHandler<GetGroceryItemTableDataQuery, TableData<GroceryItemVM>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetGroceryItemTableDataQueryHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TableData<GroceryItemVM>> Handle( GetGroceryItemTableDataQuery request, CancellationToken cancellationToken )
    {
        var query = _context.GroceryItems.AsQueryable()
            .Select( item => new GroceryItemVM
            {
                Id = item.Id,
                Name = item.Name,
                GroceryItemType = item.Type,
                RecipeCount = _context.RecipeGroceryItems.Count( rgi => rgi.GroceryItemId == item.Id ),
                GroceryListCount = _context.GroceryListItems.Count( gli => gli.GroceryItemId == item.Id ),
                Tags = _context.Tags
                    .Where( t => t.EntityType == "GroceryItem" && t.EntityId == item.Id )
                    .Select( t => t.Name )
                    .ToList()
            } );

        // search
        string searchString = (request.QueryOptions.SearchTerm ?? string.Empty).ToLower();
        if ( searchString != string.Empty )
        {
            query = query.Where( i => i.Name.ToLower().Contains( searchString ) );
        }

        // sorting
        switch ( request.QueryOptions.SortBy )
        {
            case "name":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( i => i.Name )
                    : query.OrderBy( i => i.Name );
                break;
            case "itemtype":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( i => i.GroceryItemType )
                    : query.OrderBy( i => i.GroceryItemType );
                break;
            case "recipes":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( i => i.RecipeCount )
                    : query.OrderBy( i => i.RecipeCount );
                break;
            case "grocerylists":
                query = request.QueryOptions.SortDescending
                    ? query.OrderByDescending( i => i.GroceryListCount )
                    : query.OrderBy( i => i.GroceryListCount );
                break;
            default:
                query = query.OrderBy( i => i.Name );
                break;
        }

        var total = query.Count();

        if ( request.QueryOptions.PageSize > 0 )
        {
            query = query.Skip( request.QueryOptions.Page * request.QueryOptions.PageSize )
                         .Take( request.QueryOptions.PageSize );
        }

        var groceryItems = await query
            .ToListAsync( cancellationToken );

        // get tags
        var tags = await _context.Tags.Where( t => t.EntityType == "Recipe" ).ToListAsync();
        foreach ( var item in groceryItems )
        {
            item.Tags = tags.Where( t => t.EntityId == item.Id ).Select( t => t.Name ).ToList();
        }

        return new TableData<GroceryItemVM>
        {
            TotalItems = total,
            Items = groceryItems
        };
    }
}
