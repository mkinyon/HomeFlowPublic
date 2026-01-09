using HomeFlow.Data;
using HomeFlow.Features.MealPlanning.GroceryItems;

namespace HomeFlow.Features.MealPlanning.GroceryStores
{
    public record GetGroceryStoreQuery : IRequest<List<GroceryStore>>;

    public class GetGroceryStoreQueryHandler : IRequestHandler<GetGroceryStoreQuery, List<GroceryStore>>
    {
        private readonly IHomeFlowDbContext _context;
        private readonly IMapper _mapper;

        public GetGroceryStoreQueryHandler( IHomeFlowDbContext context, IMapper mapper )
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<GroceryStore>> Handle( GetGroceryStoreQuery request, CancellationToken cancellationToken )
        {
            var groceryStoreEntities = await _context.GroceryStores
                .Include( gs => gs.GroceryStoreAisles )
                    .ThenInclude( a => a.GroceryStoreAisleGroceryItems )
                    .ThenInclude( ai => ai.GroceryItem )
                .OrderBy( gs => gs.Name )
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

            return groceryStores;
        }
    }
}
