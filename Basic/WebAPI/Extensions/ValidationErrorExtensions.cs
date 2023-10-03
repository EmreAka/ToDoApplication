using FluentValidation.Results;

namespace WebAPI.Extensions;

public static class ValidationErrorExtensions
{
    public static List<ValidationErrorResponse> ToRespose(this List<ValidationFailure> validationFailures)
    {
        return validationFailures.Select(x => new ValidationErrorResponse
        {
            Property = x.PropertyName,
            Error = x.ErrorMessage
        }).ToList() ;
    }
}

public class ValidationErrorResponse
{
    public string Property { get; set; }
    public string Error { get; set; }
}
