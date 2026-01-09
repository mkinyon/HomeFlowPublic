namespace HomeFlow.Features.Tasks.TodoLists;

public class TodoList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TodoItem> Items { get; set; } = new();
}
