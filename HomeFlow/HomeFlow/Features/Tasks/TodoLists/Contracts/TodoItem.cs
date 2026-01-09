using HomeFlow.Features.Core.FamilyMembers;

namespace HomeFlow.Features.Tasks.TodoLists;

public class TodoItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime? CompletedDateTime { get; }

    public DateTime? DueDate { get; set; }

    public int Order { get; set; }

    public FamilyMember? AssignedFamilyMember { get; set; }
}
