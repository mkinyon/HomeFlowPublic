namespace HomeFlow.Features.Tasks.TodoLists;

public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    public UpdateTodoListCommandValidator()
    {
        RuleFor(x => x.TodoList)
            .NotNull().WithMessage("Todo list is required.");

        When(x => x.TodoList != null, () =>
        {
            RuleFor(x => x.TodoList.Id)
                .NotEmpty().WithMessage("Todo list ID is required.");

            RuleFor(x => x.TodoList)
                .SetValidator(new TodoListValidator());
        });
    }
}
