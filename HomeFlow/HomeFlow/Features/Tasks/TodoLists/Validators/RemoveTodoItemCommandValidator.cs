namespace HomeFlow.Features.Tasks.TodoLists;

public class RemoveTodoItemCommandValidator : AbstractValidator<RemoveTodoItemCommand>
{
    public RemoveTodoItemCommandValidator()
    {
        RuleFor(x => x.TodoListId)
            .NotEmpty().WithMessage("Todo list ID is required.");

        RuleFor(x => x.TodoItemId)
            .NotEmpty().WithMessage("Todo item ID is required.");
    }
}
