using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record GetTodoItemsByListIdQuery(Guid TodoListId) : IRequest<List<TodoItem>>;

public class GetTodoItemsByListIdQueryHandler : IRequestHandler<GetTodoItemsByListIdQuery, List<TodoItem>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsByListIdQueryHandler(IHomeFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TodoItem>> Handle(GetTodoItemsByListIdQuery request, CancellationToken cancellationToken)
    {
        var todoItems = await _context.TodoItems
            .Include(ti => ti.AssignedFamilyMember)
            .Where(ti => ti.TodoListId == request.TodoListId)
            .OrderBy(ti => ti.Order)
            .ProjectTo<TodoItem>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return todoItems;
    }
}
