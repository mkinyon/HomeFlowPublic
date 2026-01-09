
using HomeFlow.Services;

namespace HomeFlow.Features.Tasks.TodoLists;

public interface ITodoListsService : IService<TodoList>
{
    Task<List<TodoItem>> GetTodoItemsByListIdAsync(Guid todoListId);
    Task<Guid> AddTodoItemAsync(Guid todoListId, TodoItem todoItem);
    Task RemoveTodoItemAsync(Guid todoListId, Guid todoItemId);
    Task UpdateTodoItemAsync(TodoItem todoItem);
    Task<TableData<TodoListVM>> GetVMTableDataAsync(QueryOptions options);
    Task<Guid> CopyTodoListAsync(Guid todoListId);
}

public class TodoListsService : BaseService<TodoList>, ITodoListsService
{
    public TodoListsService(IMediator mediator) : base(mediator) { }

    public override Task<Guid> CreateAsync(TodoList item) =>
        _mediator.Send(new CreateTodoListCommand(item));

    public override Task UpdateAsync(TodoList item) =>
        _mediator.Send(new UpdateTodoListCommand(item));

    public override Task DeleteAsync(Guid id) =>
        _mediator.Send(new DeleteTodoListCommand(id));

    public override Task<TodoList?> GetByIdAsync(Guid id) =>
        _mediator.Send(new GetTodoListById(id));

    public override Task<List<TodoList>> GetListAsync() =>
        _mediator.Send(new GetTodoListsQuery());

    public override Task<TableData<TodoList>> GetTableDataAsync(QueryOptions options) =>
        _mediator.Send(new GetTodoListTableDataQuery(options));

    public Task<List<TodoItem>> GetTodoItemsByListIdAsync(Guid todoListId) =>
        _mediator.Send(new GetTodoItemsByListIdQuery(todoListId));

    public Task<Guid> AddTodoItemAsync(Guid todoListId, TodoItem todoItem) =>
        _mediator.Send(new AddTodoItemCommand(todoListId, todoItem));

    public Task RemoveTodoItemAsync(Guid todoListId, Guid todoItemId) =>
        _mediator.Send(new RemoveTodoItemCommand(todoListId, todoItemId));

    public Task UpdateTodoItemAsync(TodoItem todoItem) =>
        _mediator.Send(new UpdateTodoItemCommand(todoItem));

    public Task<TableData<TodoListVM>> GetVMTableDataAsync(QueryOptions options) =>
        _mediator.Send(new GetTodoListVMTableDataQuery(options));

    public Task<Guid> CopyTodoListAsync(Guid todoListId) =>
        _mediator.Send(new CopyTodoListCommand(todoListId));
}
