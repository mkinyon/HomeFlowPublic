using HomeFlow.Data;
using HomeFlow.Services;

namespace HomeFlow.Features.Tasks.TodoLists;

public record GetTodoListTableDataQuery(QueryOptions Options) : IRequest<TableData<TodoList>>;

public class GetTodoListTableDataQueryHandler : IRequestHandler<GetTodoListTableDataQuery, TableData<TodoList>>
{
    private readonly IHomeFlowDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoListTableDataQueryHandler(IHomeFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TableData<TodoList>> Handle(GetTodoListTableDataQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TodoLists.AsQueryable();

        // Apply search
        if (!string.IsNullOrWhiteSpace(request.Options.SearchTerm))
        {
            query = query.Where(tl => tl.Name.Contains(request.Options.SearchTerm));
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(request.Options.SortBy))
        {
            query = request.Options.SortBy.ToLower() switch
            {
                "name" => request.Options.SortDescending ? query.OrderByDescending(tl => tl.Name) : query.OrderBy(tl => tl.Name),
                "created" => request.Options.SortDescending ? query.OrderByDescending(tl => tl.Created) : query.OrderBy(tl => tl.Created),
                _ => query.OrderBy(tl => tl.Name)
            };
        }
        else
        {
            query = query.OrderBy(tl => tl.Name);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((request.Options.Page - 1) * request.Options.PageSize)
            .Take(request.Options.PageSize)
            .Include(tl => tl.Items)
            .ProjectTo<TodoList>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new TableData<TodoList>
        {
            Items = items,
            TotalItems = totalItems
        };
    }
}
