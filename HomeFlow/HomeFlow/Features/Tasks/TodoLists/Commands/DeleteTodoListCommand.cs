using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record DeleteTodoListCommand(Guid Id) : IRequest;

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand>
{
    private readonly IHomeFlowDbContext _context;

    public DeleteTodoListCommandHandler(IHomeFlowDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists
            .FirstOrDefaultAsync(tl => tl.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.TodoLists.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
