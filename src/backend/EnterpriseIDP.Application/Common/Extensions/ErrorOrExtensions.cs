using EnterpriseIDP.Application.Common.Exceptions;
using ErrorOr;
using FluentValidation.Results;

namespace EnterpriseIDP.Application.Common.Extensions;

public static class ErrorOrExtensions
{
    public static T ThrowIfError<T>(this ErrorOr<T> result)
    {
        if (!result.IsError) return result.Value;

        var failures = result.Errors.Select(e => new ValidationFailure(e.Code, e.Description));
        throw new ValidationException(failures);
    }
}
