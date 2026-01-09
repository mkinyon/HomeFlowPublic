using HomeFlow.Services;

namespace HomeFlow.Features.MealPlanning.GroceryLists;

public interface IGroceryListService : IService<GroceryList>
{
    Task<PrintableGroceryListVM> GetPrintableListAsync( Guid GroceryListId, Guid? GroceryStoreId );
    Task<Guid> AddItemAsync( Guid Id, GroceryListItem item );
    Task RemoveItemAsync( Guid Id );
    Task<List<GroceryList>> GetByGroceryItemQuery( Guid groceryItemId );
}

public class GroceryListService : BaseService<GroceryList>, IGroceryListService
{
    public GroceryListService( IMediator mediator ) : base( mediator ) { }

    public override Task<Guid> CreateAsync( GroceryList item ) =>
        _mediator.Send( new CreateGroceryListCommand( item ) );

    public override Task UpdateAsync( GroceryList item ) =>
        _mediator.Send( new UpdateGroceryListCommand( item ) );

    public override Task DeleteAsync( Guid id ) =>
        _mediator.Send( new DeleteGroceryListCommand( id ) );

    public override Task<GroceryList?> GetByIdAsync( Guid id ) =>
        _mediator.Send( new GetGroceryListById( id ) );

    public override Task<List<GroceryList>> GetListAsync() =>
        _mediator.Send( new GetGroceryListQuery( true /* TODO */ ) );

    public Task<PrintableGroceryListVM> GetPrintableListAsync( Guid groceryListId, Guid? groceryStoreId ) =>
        _mediator.Send( new GetPrintableGroceryList( groceryListId, groceryStoreId ) );

    public Task<Guid> AddItemAsync( Guid id, GroceryListItem item ) =>
        _mediator.Send( new AddGroceryListItemCommand( id, item ) );

    public Task RemoveItemAsync( Guid id ) =>
        _mediator.Send( new RemoveGroceryListItemCommand( id ) );

    public override Task<TableData<GroceryList>> GetTableDataAsync( QueryOptions options ) =>
        _mediator.Send( new GetGroceryStoreTableDataQuery( options ) );

    public Task<List<GroceryList>> GetByGroceryItemQuery( Guid groceryItemId ) =>
        _mediator.Send( new GetByGroceryItemQuery( groceryItemId ) );
}
