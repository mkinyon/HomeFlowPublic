using HomeFlow.Data;

namespace HomeFlow.Features.MealPlanning.MealPlannerItems;

public record UpdateMealPlannerItemCommand( MealPlannerItem MealPlannerItem ) : IRequest;

public class UpdateMealPlannerItemCommandHandler : IRequestHandler<UpdateMealPlannerItemCommand>
{
    private readonly IHomeFlowDbContext _context;

    public UpdateMealPlannerItemCommandHandler( IHomeFlowDbContext context )
    {
        _context = context;
    }

    public async Task Handle( UpdateMealPlannerItemCommand request, CancellationToken cancellationToken )
    {
        var entity = await _context.MealPlannerItems.FirstOrDefaultAsync( i => i.Id == request.MealPlannerItem.Id, cancellationToken );

        Guard.Against.NotFound( request.MealPlannerItem.Id, entity );

        entity.Date = request.MealPlannerItem.Date;
        entity.RecipeId = request.MealPlannerItem.RecipeId;

        await _context.SaveChangesAsync( cancellationToken );
    }
}
