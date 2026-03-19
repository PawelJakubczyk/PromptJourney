using MediatR;
using Utilities.Results;

namespace Application.Abstractions;

public interface IQuery<TValue> : IRequest<Result<TValue>>;

public interface IQueryHandler<TQuery, TValue> : IRequestHandler<TQuery, Result<TValue>> where TQuery : IQuery<TValue>;