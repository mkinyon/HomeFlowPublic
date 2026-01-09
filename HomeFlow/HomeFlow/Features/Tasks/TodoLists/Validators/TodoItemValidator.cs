namespace HomeFlow.Features.Tasks.TodoLists;

public class TodoItemValidator : AbstractValidator<TodoItem>
{
    public TodoItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The todo item title is required.")
            .MaximumLength(200).WithMessage("The todo item title cannot exceed 200 characters.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0.");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<TodoItem>.CreateWithOptions((TodoItem)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select(e => e.ErrorMessage);
    };
}
