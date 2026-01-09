using HomeFlow.Features.Core.FamilyMembers;
using HomeFlow.Interfaces;
using HomeFlow.Models;

namespace HomeFlow.Features.Tasks.TodoLists;

public class TodoItemEntity : Model, IOrderable
{
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime? CompletedDateTime { get; set; }

    public DateTime? DueDate { get; set; }

    public int Order { get; set; }

    public Guid TodoListId { get; set; }

    public Guid? AssignedFamilyMemberId { get; set; }

    public virtual TodoListEntity? TodoList { get; set; }

    public virtual FamilyMemberEntity? AssignedFamilyMember { get; set; }
}
