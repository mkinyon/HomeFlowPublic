using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record RemoveTodoItemCommand(Guid TodoListId, Guid TodoItemId) : IRequest;

public class RemoveTodoItemCommandHandler : IRequestHandler<RemoveTodoItemCommand>
{
    private readonly IHomeFlowDbContext _context;

    public RemoveTodoItemCommandHandler(IHomeFlowDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItemEntity = await _context.TodoItems
            .FirstOrDefaultAsync(ti => ti.Id == request.TodoItemId && ti.TodoListId == request.TodoListId, cancellationToken);

        Guard.Against.NotFound(request.TodoItemId, todoItemEntity);

        _context.TodoItems.Remove(todoItemEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
