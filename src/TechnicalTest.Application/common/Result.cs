namespace TechnicalTest.Application.Common;

public sealed record Result<T>
{
    private Result(T? value, IReadOnlyCollection<Error> errors)
    {
        Value = value;
        Errors = errors;
    }

    public T? Value { get; }

    public IReadOnlyCollection<Error> Errors { get; }

    public bool IsSuccess => Errors.Count == 0;

    public bool IsFailure => !IsSuccess;

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, Array.Empty<Error>());
    }

    public static Result<T> Failure(params Error[] errors)
    {
        return new Result<T>(default, errors);
    }
}