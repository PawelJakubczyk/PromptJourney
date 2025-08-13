//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace Utilities.ResultPattern;

//public class Result<T>
//{
//    public T? Value { get; private set; }
//    public List<string> Errors { get; private set; } = [];
//    public bool IsSuccess => Errors.Count == 0;
//    public bool IsFailure => !IsSuccess;

//    private Result() { }

//    public static Result<T> Success(T value)
//    {
//        return new Result<T> { Value = value };
//    }

//    public static Result<T> Failure(params string[] errors)
//    {
//        return new() { Errors = [.. errors] };
//    }

//    public void AddError(string error) => Errors.Add(error);
//}


//public sealed class ResultT<TValue>
//{
//    public readonly TValue? _value;

//    public ResultT()
//    {
//    }

//    private List<string> Errors { get; set; } = [];



//    public bool IsSuccess()
//    {
//        return Errors.Count == 0;
//    }

//    public bool IsFailure()
//    {
//        return !IsSuccess();
//    }

//    public ResultT<TValue> AddError(string error)
//    {
//        Errors.Add(error);
//        return this;
//    }
//}