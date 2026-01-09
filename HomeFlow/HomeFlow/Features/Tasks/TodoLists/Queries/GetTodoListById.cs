using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record GetTodoListById(Guid Id) : IRequest<TodoList?>;

public class GetTodoListByIdHandler : IRequestHandler<GetTodoListById, TodoList?>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoListByIdHandler(IHomeFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodoList?> Handle(GetTodoListById request, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .Include(tl => tl.Items.OrderBy(ti => ti.Order))
            .ThenInclude(ti => ti.AssignedFamilyMember)
            .ProjectTo<TodoList>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(tl => tl.Id == request.Id, cancellationToken);

        return todoList;
    }
}
