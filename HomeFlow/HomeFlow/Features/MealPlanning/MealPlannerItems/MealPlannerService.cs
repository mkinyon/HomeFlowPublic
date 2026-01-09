
using HomeFlow.Services;

namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public interface IMealPlannerItemService : IService<MealPlannerItem>
{
    Task<List<MealPlannerCalendarDayVM>> GetByDateRange( DateTime? startDate, DateTime? endDate );
}

public class MealPlannerItemService : BaseService<MealPlannerItem>, IMealPlannerItemService
{
    public MealPlannerItemService( IMediator mediator ) : base( mediator ) { }

    public override Task<Guid> CreateAsync( MealPlannerItem item ) =>
        _mediator.Send( new CreateMealPlannerItemCommand( item ) );

    public override Task UpdateAsync( MealPlannerItem item ) =>
        _mediator.Send( new UpdateMealPlannerItemCommand( item ) );

    public override Task DeleteAsync( Guid id ) =>
        _mediator.Send( new DeleteMealPlannerItemCommand( id ) );

    public override Task<MealPlannerItem?> GetByIdAsync( Guid id )
    {
        throw new NotImplementedException();
    }

    public override Task<List<MealPlannerItem>> GetListAsync()
    {
        throw new NotImplementedException();
    }

    public override Task<TableData<MealPlannerItem>> GetTableDataAsync( QueryOptions options ) =>
        throw new NotImplementedException( "" );

    public Task<List<MealPlannerCalendarDayVM>> GetByDateRange( DateTime? startDate, DateTime? endDate )
    {
        if ( startDate == null || endDate == null )
        {
            throw new ArgumentNullException( "StartDate and EndDate cannot be null." );
        }

        return _mediator.Send( new GetMealPlannerItemsByDateRange( DateOnly.FromDateTime( startDate.Value ), DateOnly.FromDateTime( endDate.Value ) ) );
    }
}
