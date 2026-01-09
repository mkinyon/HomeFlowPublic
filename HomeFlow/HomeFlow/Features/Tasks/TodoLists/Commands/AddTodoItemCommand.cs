using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record AddTodoItemCommand(Guid TodoListId, TodoItem TodoItemRequest) : IRequest<Guid>;

public class AddTodoItemCommandHandler : IRequestHandler<AddTodoItemCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;

    public AddTodoItemCommandHandler(IHomeFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoListEntity = await _context.TodoLists
            .FirstOrDefaultAsync(tl => tl.Id == request.TodoListId, cancellationToken);

        Guard.Against.NotFound(request.TodoListId, todoListEntity);

        var entity = new TodoItemEntity
        {
            Title = request.TodoItemRequest.Title,
            IsCompleted = request.TodoItemRequest.IsCompleted,
            CompletedDateTime = request.TodoItemRequest.IsCompleted ? DateTime.UtcNow : null,
            DueDate = request.TodoItemRequest.DueDate,
            Order = request.TodoItemRequest.Order,
            TodoListId = todoListEntity.Id,
            AssignedFamilyMemberId = request.TodoItemRequest.AssignedFamilyMember?.Id
        };

        _context.TodoItems.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
