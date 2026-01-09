
using HomeFlow.Services;

namespace HomeFlow.Features.MealPlanning.GroceryStores;

public interface IGroceryStoreService : IService<GroceryStore> { }

public class GroceryStoreService : BaseService<GroceryStore>, IGroceryStoreService
{
    public GroceryStoreService( IMediator mediator ) : base( mediator ) { }

    public override Task<Guid> CreateAsync( GroceryStore item ) =>
        _mediator.Send( new CreateGroceryStoreCommand( item ) );

    public override Task UpdateAsync( GroceryStore item ) =>
        _mediator.Send( new UpdateGroceryStoreCommand( item ) );

    public override Task DeleteAsync( Guid id ) =>
        _mediator.Send( new DeleteGroceryStoreCommand( id ) );

    public override Task<GroceryStore?> GetByIdAsync( Guid id ) =>
        _mediator.Send( new GetGroceryStoreById( id ) );

    public override Task<List<GroceryStore>> GetListAsync() =>
        _mediator.Send( new GetGroceryStoreQuery() );

    public override Task<TableData<GroceryStore>> GetTableDataAsync( QueryOptions options ) =>
        _mediator.Send( new GetGroceryStoreTableDataQuery( options ) );
}
