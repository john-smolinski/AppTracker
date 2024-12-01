using ApplicationTracker.Common;
using ApplicationTracker.Services.Interfaces;
using ApplicationTrackerTests.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApplicationTrackerTests.Common
{
    [TestFixture]
    public class ServiceCallHandlerTests
    {
        private Mock<IServiceFactory> _mockServiceFactory;
        private Mock<IService<TestService>> _mockService;
        private Mock<ILogger> _mockLogger;
        
        public class TestService { }

        [SetUp]
        public void Setup()
        {
            _mockServiceFactory = new Mock<IServiceFactory>();
            _mockService = new Mock<IService<TestService>>();
            _mockLogger = new Mock<ILogger>();
        }

        [Test]
        public async Task HandleServiceCall_ReturnsResult_WhenActionExecutesSuccessfully()
        {
            // Setup
            _mockServiceFactory
                .Setup(factory => factory.GetService<TestService>())
                .Returns(_mockService.Object);

            var expectedResult = new OkResult();

            // Act
            var result = await ServiceCallHandler.HandleServiceCall<TestService>(
                _mockServiceFactory.Object,
                _mockLogger.Object,
                service => Task.FromResult<ActionResult>(expectedResult));

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
            _mockLogger.VerifyLog(LogLevel.Debug, "Service call result", Times.Once());

        }

        [Test]
        public async Task HandleServiceCall_ReturnsInternalServerError_WhenActionReturnsNull()
        {
            // Setup
            _mockServiceFactory
                .Setup(factory => factory.GetService<TestService>())
                .Returns(_mockService.Object);

            // Act
            var result = await ServiceCallHandler.HandleServiceCall<TestService>(
                _mockServiceFactory.Object,
                _mockLogger.Object,
                service => Task.FromResult<ActionResult>(null));

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());

            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult, Is.Not.Null, "ObjectResult is null");
                Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult.Value, Is.InstanceOf<ErrorResponse>());
                
                var errorResponse = objectResult.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null, "ErrorResponse is null");
                Assert.That(errorResponse!.Message, Is.EqualTo("Unexpected error"));
                Assert.That(errorResponse.Detail, Is.EqualTo("The service call returned a null result."));
            });
            _mockLogger.VerifyLog(LogLevel.Error, "The service call returned a null ActionResult", Times.Once());
        }

        [Test]
        public async Task HandleServiceCall_ReturnsInternalServerError_WhenServiceFactoryThrowsInvalidOperationException()
        {
            // Setup
            _mockServiceFactory
                .Setup(factory => factory.GetService<TestService>())
                .Throws(new InvalidOperationException("Service not found"));

            // Act
            var result = await ServiceCallHandler.HandleServiceCall<TestService>(
                _mockServiceFactory.Object,
                _mockLogger.Object,
                service => Task.FromResult<ActionResult>(new OkResult()));

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());

            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult, Is.Not.Null, "ObjectResult is null");
                Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult.Value, Is.InstanceOf<ErrorResponse>());
                
                var errorResponse = objectResult.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null, "ErrorResponse is null");
                Assert.That(errorResponse!.Message, Is.EqualTo("Service not found"));
            });

            _mockLogger.VerifyLog(LogLevel.Error, "Service not found for type", Times.Once());
        }

        [Test]
        public async Task HandleServiceCall_ReturnsInternalServerError_WhenServiceFactoryThrowsArgumentException()
        {
            // Setup
            _mockServiceFactory
                .Setup(factory => factory.GetService<TestService>())
                .Throws(new ArgumentException("Service not registered"));

            // Act
            var result = await ServiceCallHandler.HandleServiceCall<TestService>(
                _mockServiceFactory.Object,
                _mockLogger.Object,
                service => Task.FromResult<ActionResult>(new OkResult()));

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());

            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult, Is.Not.Null, "ObjectResult is null");
                Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult.Value, Is.InstanceOf<ErrorResponse>());
                
                var errorResponse = objectResult.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null, "ErrorResponse is null");
                Assert.That(errorResponse!.Message, Is.EqualTo("Service not registered in ServiceFactory"));
            });
            _mockLogger.VerifyLog(LogLevel.Error, "Service not registered in ServiceFactory for type", Times.Once());
        }

        [Test]
        public async Task HandleServiceCall_ReturnsInternalServerError_WhenUnhandledExceptionOccurs()
        {
            // Setup
            _mockServiceFactory
                .Setup(factory => factory.GetService<TestService>())
                .Returns(_mockService.Object);

            // Act
            var result = await ServiceCallHandler.HandleServiceCall<TestService>(
                _mockServiceFactory.Object,
                _mockLogger.Object,
                service => throw new Exception("Unhandled exception"));

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());

            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult, Is.Not.Null, "ObjectResult is null");
                Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult.Value, Is.InstanceOf<ErrorResponse>());

                var errorResponse = objectResult.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null, "ErrorResponse is null");
                Assert.That(errorResponse!.Message, Is.EqualTo("Unhandled exception occurred"));
            });
            _mockLogger.VerifyLog(LogLevel.Error, "Unhandled exception occurred while processing", Times.Once());
        }

        [Test]
        public void HandleServiceCall_ThrowsArgumentNullException_WhenActionIsNull()
        {
#pragma warning disable CS8625
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await ServiceCallHandler.HandleServiceCall<TestService>(_mockServiceFactory.Object, _mockLogger.Object, null));
#pragma warning restore CS8625
        }
    }
}
