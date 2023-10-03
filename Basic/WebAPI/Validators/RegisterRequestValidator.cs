using FluentValidation;
using WebAPI.ModelViews;

namespace WebAPI.Validators;

public class RegisterRequestValidator: AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().NotNull().MinimumLength(8);
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
    }
}
