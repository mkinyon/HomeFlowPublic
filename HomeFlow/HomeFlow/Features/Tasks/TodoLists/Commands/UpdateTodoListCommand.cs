using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record UpdateTodoListCommand(TodoList TodoList) : IRequest;

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand>
{
    private readonly IHomeFlowDbContext _context;

    public UpdateTodoListCommandHandler(IHomeFlowDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists
            .FirstOrDefaultAsync(tl => tl.Id == request.TodoList.Id, cancellationToken);

        Guard.Against.NotFound(request.TodoList.Id, entity);

        entity.Name = request.TodoList.Name;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
