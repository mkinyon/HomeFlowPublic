using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public record CreateMealPlannerItemCommand( MealPlannerItem MealPlannerItem ) : IRequest<Guid>;

public class CreateMealPlannerItemCommandHandler : IRequestHandler<CreateMealPlannerItemCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;

    public CreateMealPlannerItemCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task<Guid> Handle( CreateMealPlannerItemCommand request, CancellationToken cancellationToken )
    {
        var entity = new MealPlannerItemEntity
        {
            Date = request.MealPlannerItem.Date,
            RecipeId = request.MealPlannerItem.RecipeId
        };

        _context.MealPlannerItems.Add( entity );

        await _context.SaveChangesAsync( cancellationToken );

        return entity.Id;
    }
}
