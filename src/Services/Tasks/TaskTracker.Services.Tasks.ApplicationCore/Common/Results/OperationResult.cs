namespace TaskTracker.Services.Tasks.ApplicationCore.Common.Results;

public class OperationResult<T>
{
    private readonly T? _value;

    public bool Succeeded { get; }

    public T Value
    {
        get => !Succeeded ? throw new ArgumentException("There is no value for failure.") : _value!;
        private init => _value = value;
    }

    public Error Error { get; }

    private OperationResult(T value)
    {
        Value = value;
        Succeeded = true;
        Error = Error.None;
    }

    private OperationResult(Error error)
    {
        if (error == Error.None)
            throw new ArgumentException("Invalid error.");
        
        Succeeded = false;
        Error = error;
    }

    public static implicit operator OperationResult<T>(T result) => new(result);
    public static implicit operator OperationResult<T>(Error error) => new(error);
}