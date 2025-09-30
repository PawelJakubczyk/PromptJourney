using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using Utilities.Errors;
using static Presentation.Constants.StatusPriority;

namespace Presentation.Controllers.ControllersUtilities
{
    public sealed class Pipeline<T>
    {
        public Result<T> Result { get; }
        public Error<ILayer>? MainError { get; private set; }
        public IActionResult? Response { get; private set; }

        public Pipeline(Result<T> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            Result = result;
        }

        public Pipeline<T> PrepareErrorResponse(Func<Error<ILayer>, IEnumerable<object>, object>? bodyFactory = null)
        {
            var layerErrors = Result.Errors.OfType<Error<ILayer>>().ToList();
            MainError = layerErrors.Count > 0
                ? PickHighestPriorityErrorInternal(layerErrors)
                : new Error<ILayer>("Unknown error", StatusCodes.Status500InternalServerError);

            
            // build details list
            var detailsList = new List<object>();
            foreach (var error in Result.Errors)
            {
                int? code = null;
                if (error is Error<ILayer> el)
                {
                    code = el.ErrorCode;
                }

                detailsList.Add(new
                {
                    message = error.Message,
                    code = code,
                    metadata = error.Metadata
                });
            }

            object body;
            if (bodyFactory != null)
            {
                body = bodyFactory(MainError, detailsList);
            }
            else
            {
                body = new
                {
                    MainError = new { code = MainError.ErrorCode, message = MainError.Message },
                    details = detailsList
                };
            }

            Response = new ObjectResult(body)
            {
                StatusCode = MainError.ErrorCode
            };

            return this;
        }

        public Pipeline<T> PrepareOKResponse(Func<T?, IActionResult>? bodyFactory = null)
        {
            T? payload = default;
            if (Result.IsSuccess)
            {
                payload = Result.Value;
            }

            Response = bodyFactory != null
                ? bodyFactory(payload)
                : new OkObjectResult(payload);

            return this;
        }

        public Task<IActionResult> ToActionResultAsync()
        {
            if (Response != null)
            {
                return Task.FromResult(Response);
            }

            if (Result.IsFailed)
            {
                Error<ILayer> main = MainError ?? DetermineFallbackMainError();
                var details = Result.Errors.Select(e => new { message = e.Message, metadata = e.Metadata }).ToList();
                var body = new { MainError = new { code = main.ErrorCode, message = main.Message }, details = details };
                return Task.FromResult<IActionResult>(new ObjectResult(body) { StatusCode = main.ErrorCode });
            }

            return Task.FromResult<IActionResult>(new OkResult());
        }

        private Error<ILayer> DetermineFallbackMainError()
        {
            var layerErrors = Result.Errors.OfType<Error<ILayer>>().ToList();
            if (layerErrors.Count > 0)
            {
                return PickHighestPriorityErrorInternal(layerErrors);
            }

            return new Error<ILayer>("Unknown error", StatusCodes.Status500InternalServerError);
        }

        public static Error<ILayer> PickHighestPriorityErrorInternal(List<Error<ILayer>> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));
            if (errors.Count == 0) throw new ArgumentException("errors must contain at least one item", nameof(errors));

            var ordered = errors
                .OrderBy(e =>
                {
                    if (StatusPriorityDict.TryGetValue(e.ErrorCode, out var priority))
                    {
                        return priority;
                    }

                    return int.MaxValue;
                })
                .ToList();

            return ordered.First();
        }
    }

    public static class PipelineExtensions
    {
        public static async Task<Pipeline<T>> AsPipeline<T>(this Task<Result<T>> sourceTask)
        {
            if (sourceTask == null) throw new ArgumentNullException(nameof(sourceTask));
            var result = await sourceTask.ConfigureAwait(false);
            return new Pipeline<T>(result);
        }

        public static async Task<Pipeline<T>> IfErrors<T>(this Task<Result<T>> sourceTask, Func<Pipeline<T>, Pipeline<T>> branch)
        {
            if (sourceTask == null) throw new ArgumentNullException(nameof(sourceTask));
            if (branch == null) throw new ArgumentNullException(nameof(branch));

            var pipeline = await sourceTask.AsPipeline().ConfigureAwait(false);
            if (pipeline.Result.IsFailed)
            {
                return branch(pipeline);
            }

            return pipeline;
        }

        public static async Task<Pipeline<T>> Else<T>(this Task<Pipeline<T>> pipelineTask, Func<Pipeline<T>, Pipeline<T>> branch)
        {
            if (pipelineTask == null) throw new ArgumentNullException(nameof(pipelineTask));
            if (branch == null) throw new ArgumentNullException(nameof(branch));

            var pipeline = await pipelineTask.ConfigureAwait(false);
            if (pipeline.Result.IsFailed)
            {
                return pipeline;
            }

            return branch(pipeline);
        }

        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Pipeline<T>> pipelineTask)
        {
            if (pipelineTask == null) throw new ArgumentNullException(nameof(pipelineTask));
            var pipeline = await pipelineTask.ConfigureAwait(false);
            return await pipeline.ToActionResultAsync().ConfigureAwait(false);
        }

        public static Task<IActionResult> ToActionResultAsync<T>(this Pipeline<T> pipeline)
        {
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            return pipeline.ToActionResultAsync();
        }
    }
}
