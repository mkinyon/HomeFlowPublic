using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public record GetPrintableGroceryList( Guid GroceryListId, Guid? GroceryStoreId ) : IRequest<PrintableGroceryListVM>;

public class GetPrintableGroceryListHandler : IRequestHandler<GetPrintableGroceryList, PrintableGroceryListVM>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetPrintableGroceryListHandler( IHomeFlowDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PrintableGroceryListVM> Handle( GetPrintableGroceryList request, CancellationToken cancellationToken )
    {
        var groceryListEntity = await _context.GroceryLists
            .Include( gl => gl.Items )
            .ThenInclude( i => i.SourceRecipe )
            .Include( gl => gl.Items )
            .ThenInclude( i => i.RecipeGroceryItem )
            .FirstOrDefaultAsync( gs => gs.Id == request.GroceryListId, cancellationToken );

        Guard.Against.NotFound( request.GroceryListId, groceryListEntity );

        var groceryStoreEntity = await _context.GroceryStores
            .Include( gs => gs.GroceryStoreAisles )
            .ThenInclude( a => a.GroceryStoreAisleGroceryItems )
            .ThenInclude( ai => ai.GroceryItem )
            .FirstOrDefaultAsync( gs => gs.Id == request.GroceryStoreId, cancellationToken );

        PrintableGroceryListVM groceryList = new PrintableGroceryListVM
        {
            Name = groceryListEntity.Name,
            Categories = new List<PrintableGroceryCategoryVM>() // Initialize Categories
        };

        // If a grocery store is provided, organize the grocery items by aisle
        if ( groceryStoreEntity != null )
        {
            groceryList.StoreName = groceryStoreEntity.Name;
            groceryList.StoreLocation = groceryStoreEntity.Location;

            var miscCat = new PrintableGroceryCategoryVM
            {
                Name = "Miscellaneous",
                Order = 99,
                GroceryItems = new List<PrintableGroceryItemVM>() // Initialize GroceryItems
            };

            foreach ( var groceryListItem in groceryListEntity.Items )
            {
                var groceryItemVM = new PrintableGroceryItemVM
                {
                    Name = groceryListItem.Text,
                };

                // Find the aisle for the grocery item
                var aisle = groceryStoreEntity.GroceryStoreAisles
                    .SelectMany( a => a.GroceryStoreAisleGroceryItems )
                    .FirstOrDefault( ai => ai.GroceryItem.Id == groceryListItem.GroceryItemId ||
                                     ai.GroceryItem.Id == groceryListItem.RecipeGroceryItem?.GroceryItemId )?.GroceryStoreAisle;


                if ( aisle != null )
                {
                    var category = groceryList.Categories
                        .FirstOrDefault( c => c.Name == aisle.Name );

                    if ( category == null )
                    {
                        category = new PrintableGroceryCategoryVM
                        {
                            Name = aisle.Name,
                            Order = aisle.Order,
                            GroceryItems = new List<PrintableGroceryItemVM>() // Initialize GroceryItems
                        };
                        groceryList.Categories.Add( category );
                    }

                    category.GroceryItems.Add( groceryItemVM );
                }
                else
                {
                    miscCat.GroceryItems.Add( groceryItemVM );
                }
            }

            if ( miscCat.GroceryItems.Any() )
            {
                groceryList.Categories.Add( miscCat );
            }
        }
        else
        {
            // Organize by recipe 
            var miscCat = new PrintableGroceryCategoryVM
            {
                Name = "Miscellaneous",
                Order = 99,
                GroceryItems = new List<PrintableGroceryItemVM>() // Initialize GroceryItems
            };

            foreach ( var groceryListItem in groceryListEntity.Items )
            {
                if ( groceryListItem.SourceRecipeId == null )
                {
                    var groceryItemVM = new PrintableGroceryItemVM
                    {
                        Name = groceryListItem.Text,
                    };

                    miscCat.GroceryItems.Add( groceryItemVM );
                }
                else
                {
                    var groceryItemVM = new PrintableGroceryItemVM
                    {
                        Name = groceryListItem.Text,
                    };

                    var recipeCat = groceryList.Categories
                        .FirstOrDefault( c => c.Name == groceryListItem.SourceRecipe!.Name );

                    if ( recipeCat == null )
                    {
                        var cat = new PrintableGroceryCategoryVM
                        {
                            Name = groceryListItem.SourceRecipe!.Name
                        };

                        cat.GroceryItems.Add( groceryItemVM );
                        groceryList.Categories.Add( cat );
                    }
                    else
                    {
                        recipeCat.GroceryItems.Add( groceryItemVM );
                    }
                }
            }

            if ( miscCat.GroceryItems.Any() )
            {
                groceryList.Categories.Add( miscCat );
            }
        }

        // sort
        groceryList.Categories = groceryList.Categories
            .OrderBy( c => c.Order )
            .ThenBy( c => c.Name )
            .ToList();

        return groceryList;
    }
}
