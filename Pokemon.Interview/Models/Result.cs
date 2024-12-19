using Pokemon.Interview.Enums;

public class Result<T>
{
    public bool IsSuccess => Status == ResultStatus.Ok;

    public T Data { get; }

    public string ErrorMessage { get; }

    public bool HasInvalid => Status == ResultStatus.Invalid;

    public ResultStatus Status { get; private set; } = ResultStatus.Ok;


    private Result(T data, ResultStatus status, string? errorMessage)
    {
        Data = data;
        Status = status;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Success(T data) => new Result<T>(data, ResultStatus.Ok, null);

    public static Result<T> Invalid(string errorMessage) => new Result<T>(default, ResultStatus.Invalid, errorMessage);

    public static Result<T> NotFound(string errorMessage) => new Result<T>(default, ResultStatus.NotFound, errorMessage);
}
