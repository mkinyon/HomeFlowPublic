using HomeFlow.Models;

namespace HomeFlow.Features.Tasks.TodoLists;

public class TodoListEntity : Model
{
    public string Name { get; set; } = string.Empty;

    public List<TodoItemEntity> Items { get; set; } = new();
}
