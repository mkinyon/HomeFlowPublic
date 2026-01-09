namespace HomeFlow.Features.Tasks.TodoLists;

public class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
    public CreateTodoListCommandValidator()
    {
        RuleFor(x => x.TodoListRequest)
            .NotNull().WithMessage("Todo list request is required.");

        When(x => x.TodoListRequest != null, () =>
        {
            RuleFor(x => x.TodoListRequest)
                .SetValidator(new TodoListValidator());
        });
    }
}
