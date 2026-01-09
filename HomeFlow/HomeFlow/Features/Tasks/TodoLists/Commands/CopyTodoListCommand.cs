using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record CopyTodoListCommand(Guid TodoListId) : IRequest<Guid>;

public class CopyTodoListCommandHandler : IRequestHandler<CopyTodoListCommand, Guid>
{
    private readonly IHomeFlowDbContext _context;

    public CopyTodoListCommandHandler(IHomeFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CopyTodoListCommand request, CancellationToken cancellationToken)
    {
        // Get the original todo list with all its items
        var originalTodoList = await _context.TodoLists
            .Include(tl => tl.Items)
            .FirstOrDefaultAsync(tl => tl.Id == request.TodoListId, cancellationToken);

        Guard.Against.NotFound(request.TodoListId, originalTodoList);

        // Create the new todo list entity
        var newTodoListEntity = new TodoListEntity
        {
            Name = $"{originalTodoList.Name} (Copy)"
        };

        _context.TodoLists.Add(newTodoListEntity);
        await _context.SaveChangesAsync(cancellationToken);

        // Copy all items from the original list
        foreach (var originalItem in originalTodoList.Items.OrderBy(i => i.Order))
        {
            var newItemEntity = new TodoItemEntity
            {
                Title = originalItem.Title,
                IsCompleted = false, // Reset completion status
                CompletedDateTime = null, // Remove completion date
                DueDate = null, // Remove due date
                Order = originalItem.Order,
                TodoListId = newTodoListEntity.Id,
                AssignedFamilyMemberId = originalItem.AssignedFamilyMemberId // Keep assignment
            };

            _context.TodoItems.Add(newItemEntity);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return newTodoListEntity.Id;
    }
}
