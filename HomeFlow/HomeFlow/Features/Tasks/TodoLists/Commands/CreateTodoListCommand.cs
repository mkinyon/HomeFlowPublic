using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record CreateTodoListCommand(TodoList TodoListRequest) : IRequest<Guid>;

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;

    public CreateTodoListCommandHandler(IHomeFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoListEntity
        {
            Name = request.TodoListRequest.Name
        };

        _context.TodoLists.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
