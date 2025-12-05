namespace Inno.CorePlatform.Finance.Application.Common;

/// <summary>
/// 操作结果
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public string? Code { get; }

    protected Result(bool isSuccess, string? error = null, string? code = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Code = code;
    }

    public static Result Ok() => new(true);
    public static Result Fail(string error) => new(false, error);
    public static Result Fail(string code, string error) => new(false, error, code);

    public static Result<T> Ok<T>(T value) => new(value, true);
    public static Result<T> Fail<T>(string error) => new(default, false, error);
    public static Result<T> Fail<T>(string code, string error) => new(default, false, error, code);
}

/// <summary>
/// 带数据的操作结果
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }

    internal Result(T? value, bool isSuccess, string? error = null, string? code = null)
        : base(isSuccess, error, code)
    {
        Value = value;
    }
}
