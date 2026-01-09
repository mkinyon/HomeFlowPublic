using HomeFlow.Data;
using HomeFlow.Services;

namespace HomeFlow.Features.Tasks.TodoLists;

public record GetTodoListVMTableDataQuery(QueryOptions Options) : IRequest<TableData<TodoListVM>>;

public class GetTodoListVMTableDataQueryHandler : IRequestHandler<GetTodoListVMTableDataQuery, TableData<TodoListVM>>
{
    private readonly IHomeFlowDbContext _context;

    public GetTodoListVMTableDataQueryHandler(IHomeFlowDbContext context)
    {
        _context = context;
    }

    public async Task<TableData<TodoListVM>> Handle(GetTodoListVMTableDataQuery request, CancellationToken cancellationToken)
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
                "modified" => request.Options.SortDescending ? query.OrderByDescending(tl => tl.Modified) : query.OrderBy(tl => tl.Modified),
                "totalitems" => request.Options.SortDescending ? query.OrderByDescending(tl => tl.Items.Count) : query.OrderBy(tl => tl.Items.Count),
                "completeditems" => request.Options.SortDescending ? query.OrderByDescending(tl => tl.Items.Count(i => i.IsCompleted)) : query.OrderBy(tl => tl.Items.Count(i => i.IsCompleted)),
                _ => query.OrderBy(tl => tl.Name)
            };
        }
        else
        {
            query = query.OrderBy(tl => tl.Name);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        // Apply pagination and get data
        var todoListVMs = await query
            .Include(tl => tl.Items)
            .Skip((request.Options.Page - 1) * request.Options.PageSize)
            .Take(request.Options.PageSize)
            .Select(tl => new TodoListVM
            {
                Id = tl.Id,
                Name = tl.Name,
                TotalItems = tl.Items.Count,
                CompletedItems = tl.Items.Count(i => i.IsCompleted),
                PendingItems = tl.Items.Count(i => !i.IsCompleted),
                Created = tl.Created,
                LastModified = tl.Modified
            })
            .ToListAsync(cancellationToken);

        return new TableData<TodoListVM>
        {
            Items = todoListVMs,
            TotalItems = totalItems
        };
    }
}
