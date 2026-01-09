namespace HomeFlow.Features.Tasks.TodoLists;

public class TodoListVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int CompletedItems { get; set; }
    public int PendingItems { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastModified { get; set; }
}
