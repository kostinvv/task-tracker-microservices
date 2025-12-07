namespace TaskTracker.Services.Shared.Results;

public sealed class ResultT<TValue> : Result
{
    private readonly TValue? _value;
    
    private ResultT(TValue value)
    {
        _value = value;
    }
    
    private ResultT(Error error) : base(error)
    {
        _value = default;
    }
    
    public TValue Value =>
        IsSuccess ? _value! : throw new InvalidOperationException("Value can not be accessed when IsSuccess is false");
    
    public static implicit operator ResultT<TValue>(Error error) => new(error);
    
    public static implicit operator ResultT<TValue>(TValue value) => new(value);
    
    public static ResultT<TValue> Success(TValue value) => new(value);
    
    public new static ResultT<TValue> Failure(Error error) => new(error);
}