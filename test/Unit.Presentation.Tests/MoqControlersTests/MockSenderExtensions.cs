using FluentResults;
using MediatR;
using Moq;

namespace Unit.Presentation.Tests.MoqControlersTests
{
    public static class MockSenderExtensions
    {
        /// <summary>
        /// Setup a Mock<ISender> to return the provided Result<TResponse> for any request of type IRequest<Result<TResponse>>.
        /// Returns the same mock to allow fluent chaining.
        /// </summary>
        public static Mock<ISender> SetupSendReturns<TResponse>(this Mock<ISender> mock, Result<TResponse> result)
        {
            mock.Setup(s => s.Send(It.IsAny<IRequest<Result<TResponse>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return mock;
        }

        /// <summary>
        /// Setup a Mock<ISender> to return the provided Result<TResponse> for a specific request type TRequest that implements IRequest<Result<TResponse>>.
        /// </summary>
        public static Mock<ISender> SetupSendReturnsForRequest<TRequest, TResponse>(this Mock<ISender> mock, Result<TResponse> result)
            where TRequest : IRequest<Result<TResponse>>
        {
            mock.Setup(s => s.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return mock;
        }

        /// <summary>
        /// Setup a Mock<ISender> to throw OperationCanceledException for any Send invocation (useful for cancellation tests).
        /// </summary>
        public static Mock<ISender> SetupSendThrowsOperationCanceled(this Mock<ISender> mock)
        {
            mock.Setup(s => s.Send(It.IsAny<IRequest<object>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
            return mock;
        }

        /// <summary>
        /// Setup a Mock<ISender> to throw OperationCanceledException for any Send invocation for IRequest<Result<TResponse>>.
        /// </summary>
        public static Mock<ISender> SetupSendThrowsOperationCanceledForAny<TResponse>(this Mock<ISender> mock)
        {
            mock.Setup(s => s.Send(It.IsAny<IRequest<Result<TResponse>>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
            return mock;
        }

        /// <summary>
        /// Convenience helper to create a controller using a Mock<ISender> factory.
        /// Usage: var controller = MockHelpers.BuildController(senderMock, s => new MyController(s));
        /// </summary>
        public static TController BuildController<TController>(this Mock<ISender> mock, Func<ISender, TController> factory)
        {
            return factory(mock.Object);
        }
    }
}
