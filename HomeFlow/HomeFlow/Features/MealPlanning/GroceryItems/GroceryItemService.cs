using HomeFlow.Services;

namespace HomeFlow.Features.MealPlanning.GroceryItems;

public interface IGroceryItemService : IService<GroceryItem>
{
    Task<TableData<GroceryItemVM>> GetVMTableDataAsync( QueryOptions options );

    Task<List<GroceryItemVM>> GetVMListAsync();
    Task MergeGroceryItemsAsync( GroceryItem newGroceryItem, List<GroceryItem> groceryItemsToMerge );
}

public class GroceryItemService : BaseService<GroceryItem>, IGroceryItemService
{
    public GroceryItemService( IMediator mediator ) : base( mediator ) { }

    public override Task<Guid> CreateAsync( GroceryItem item ) =>
        _mediator.Send( new CreateGroceryItemCommand( item ) );

    public override Task UpdateAsync( GroceryItem item ) =>
        _mediator.Send( new UpdateGroceryItemCommand( item ) );

    public override Task DeleteAsync( Guid id ) =>
        _mediator.Send( new DeleteGroceryItemCommand( id ) );

    public override Task<GroceryItem?> GetByIdAsync( Guid id ) =>
        _mediator.Send( new GetGroceryItemById( id ) );

    public override Task<List<GroceryItem>> GetListAsync() =>
        _mediator.Send( new GetGroceryItemQuery() );

    public override Task<TableData<GroceryItem>> GetTableDataAsync( QueryOptions options ) =>
        throw new NotImplementedException( "Use GetTableDataAsync with GroceryItemVM instead." );

    public Task<TableData<GroceryItemVM>> GetVMTableDataAsync( QueryOptions options ) =>
        _mediator.Send( new GetGroceryItemTableDataQuery( options ) );

    public Task<List<GroceryItemVM>> GetVMListAsync() =>
        _mediator.Send( new GetGroceryItemListVM() );

    public Task MergeGroceryItemsAsync( GroceryItem newGroceryItem, List<GroceryItem> groceryItemsToMerge ) =>
        _mediator.Send( new MergeGroceryItemsCommand( newGroceryItem, groceryItemsToMerge ) );
}
