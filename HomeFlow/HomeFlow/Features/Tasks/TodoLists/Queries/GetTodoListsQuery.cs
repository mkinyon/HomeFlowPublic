using HomeFlow.Data;

namespace HomeFlow.Features.Tasks.TodoLists;

public record GetTodoListsQuery : IRequest<List<TodoList>>;

public class GetTodoListsQueryHandler : IRequestHandler<GetTodoListsQuery, List<TodoList>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoListsQueryHandler(IHomeFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TodoList>> Handle(GetTodoListsQuery request, CancellationToken cancellationToken)
    {
        var todoLists = await _context.TodoLists
            .Include(tl => tl.Items.OrderBy(ti => ti.Order))
            .ThenInclude(ti => ti.AssignedFamilyMember)
            .ProjectTo<TodoList>(_mapper.ConfigurationProvider)
            .OrderBy(tl => tl.Name)
            .ToListAsync(cancellationToken);

        return todoLists;
    }
}
