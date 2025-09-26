//using FluentResults;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Utilities.Constants;
//using Utilities.Errors;

//namespace Persistence.Middlewatre;

//public class ExceptionToFluentResultBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
//{
//    private readonly ILogger<ExceptionToFluentResultBehavior<TRequest, TResponse>> _logger;

//    public ExceptionToFluentResultBehavior(ILogger<ExceptionToFluentResultBehavior<TRequest, TResponse>> logger)
//    {
//        _logger = logger;
//    }

//    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<TResponse>> next)
//    {
//        try
//        {
//            return await next();
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Unhandled exception in request pipeline. RequestType: {RequestType}", typeof(TRequest).FullName);

//            var error = new Error<PersistenceLayer>($"Unhandled exception: {ex.Message}", StatusCodes.Status500InternalServerError);

//            return Result.Fail<TResponse>(error);
//        }
//    }
//}
