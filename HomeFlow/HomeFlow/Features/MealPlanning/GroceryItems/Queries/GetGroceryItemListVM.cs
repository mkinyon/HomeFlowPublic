using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public record GetGroceryItemListVM : IRequest<List<GroceryItemVM>>;

public class GetGroceryItemListVMHandler : IRequestHandler<GetGroceryItemListVM, List<GroceryItemVM>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetGroceryItemListVMHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GroceryItemVM>> Handle( GetGroceryItemListVM request, CancellationToken cancellationToken )
    {
        var groceryItems = await _context.GroceryItems
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
            } )
            .OrderBy( i => i.Name )
            .ToListAsync( cancellationToken );

        return groceryItems;
    }
}
