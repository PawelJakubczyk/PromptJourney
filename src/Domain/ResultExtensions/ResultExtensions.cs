//using FluentResults;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Domain.ResultExtensions;

//public static class ResultExtensions
//{
//    public static Result FailNotFound(this Result _, string errorMessage)
//    {
//        var result = new Result();
//        result.WithError(Settings.ErrorFactory(errorMessage));
//        return result;
//    }

//    public static Result FailInvalid(this Result _, string errorMessage)
//    {
//        var result = new Result();
//        result.WithError(Settings.ErrorFactory(errorMessage));
//        return result;
//    }
//}
