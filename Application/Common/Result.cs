namespace Application.Common;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string Error { get; protected set; } = string.Empty;
    public ErrorType ErrorType { get; protected set; } = ErrorType.Failure;

    public static Result Success(string message = "")
        => new()
        {
            IsSuccess = true,
            Error = message
        };

    public static Result Failure(
        string message,
        ErrorType errorType = ErrorType.Failure)
        => new()
        {
            IsSuccess = false,
            Error = message,
            ErrorType = errorType
        };
}

public class Result<T> : Result
{
    public T? Value { get; private set; }

    public static Result<T> Success(
        T value,
        string message = "")
        => new()
        {
            IsSuccess = true,
            Value = value,
            Error = message
        };

    public static Result<T> Failure(
        string message,
        ErrorType errorType = ErrorType.Failure)
        => new()
        {
            IsSuccess = false,
            Error = message,
            ErrorType = errorType
        };
}