using FluentResults;
using MediatR;

namespace Application.Abstractions;

public interface ICommand<TValue> : IRequest<Result<TValue>>;

public interface ICommandHandler<TCommand, TValue> : IRequestHandler<TCommand, Result<TValue>> where TCommand : ICommand<TValue>;