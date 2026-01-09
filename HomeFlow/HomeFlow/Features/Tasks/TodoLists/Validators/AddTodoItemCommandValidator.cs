namespace HomeFlow.Features.Tasks.TodoLists;

public class AddTodoItemCommandValidator : AbstractValidator<AddTodoItemCommand>
{
    public AddTodoItemCommandValidator()
    {
        RuleFor(x => x.TodoListId)
            .NotEmpty().WithMessage("Todo list ID is required.");

        RuleFor(x => x.TodoItemRequest)
            .NotNull().WithMessage("Todo item request is required.");

        When(x => x.TodoItemRequest != null, () =>
        {
            RuleFor(x => x.TodoItemRequest)
                .SetValidator(new TodoItemValidator());
        });
    }
}
