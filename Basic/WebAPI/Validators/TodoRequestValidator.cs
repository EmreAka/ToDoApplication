using FluentValidation;
using WebAPI.ModelViews;

namespace WebAPI.Validators;

public class TodoRequestValidator : AbstractValidator<ToDoRequest>
{
    public TodoRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().NotNull();
        RuleFor(x => x.Description).NotEmpty().NotNull();
        RuleFor(x => x.Deadline).NotEmpty().NotNull()
            .Must(x => x.ToUniversalTime() > DateTime.UtcNow)
            .WithMessage("'Deadline' must be greater than today");
    }
}
