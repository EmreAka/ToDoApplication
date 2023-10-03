using FluentValidation;
using WebAPI.Extensions;

namespace WebAPI.Filters;

public class ValidatorFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidatorFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (context.Arguments
            .SingleOrDefault(x => x.GetType() == typeof(T)) is not T validatable)
            return Results.BadRequest();

        var validationResult = await _validator.ValidateAsync(validatable);

        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.ToRespose());

        //Before endpoint
        var result = await next(context);
        //After endpoint
        return result;
    }
}
