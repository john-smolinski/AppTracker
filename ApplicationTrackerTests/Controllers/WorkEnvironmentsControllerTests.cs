using ApplicationTracker.Common;
using ApplicationTracker.Controllers;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services;
using ApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;

namespace ApplicationTrackerTests.Controllers
{
    [TestFixture]
    public class WorkEnvironmentsControllerTests
    {
        private Mock<IService<WorkEnvironmentDto>> _mockService;
        private Mock<ILogger<WorkEnvironmentsController>> _mockLogger;
        private WorkEnvironmentsController _controller;

        private List<WorkEnvironmentDto> _workEnvironments;
        private ServiceFactory _serviceFactory;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IService<WorkEnvironmentDto>>();
            _mockLogger = new Mock<ILogger<WorkEnvironmentsController>>();

            _workEnvironments =
            [
                new() { Id = 1, Name = "Test 1" },
                new() { Id = 2, Name = "Test 2" },
                new() { Id = 3, Name = "Test 3" },
                new() { Id = 4, Name = "Test 4" }
            ];

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(WorkEnvironmentService)))
                .Returns(_mockService.Object);

            _serviceFactory = new ServiceFactory(mockServiceProvider.Object);
            _controller = new WorkEnvironmentsController(_serviceFactory, _mockLogger.Object);
        }

        [Test]
        public async Task GetEnvironments_ReturnsOk_WhenEnvironmentsExist()
        {
            // Setup 
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync(_workEnvironments);

            // Act
            var result = await _controller.GetEnvironments();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);

            var returnedEnvironments = okResult.Value as IEnumerable<WorkEnvironmentDto>;
            Assert.That(returnedEnvironments, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedEnvironments.Count(), Is.EqualTo(4));
                Assert.That(returnedEnvironments.First().Name, Does.StartWith($"Test"));
            });
        }

        [Test]
        public async Task GetEnvironments_ReturnsNotFound_WhenNoEnvironmentsExist()
        {
            // Setup
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync([]);

            // Act
            var result = await _controller.GetEnvironments();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse.Message, Is.EqualTo("WorkEnvironments missing"));
        }

        [Test]
        public async Task GetWorkEnvironment_ReturnsOk_WhenEnvironmentExists()
        {
            // Setup
            var testId = 1;
            
            _mockService
                .Setup(service => service.ExistsAsync(testId))
                .ReturnsAsync(true);

            _mockService
                .Setup(service => service.GetByIdAsync(testId))
                .ReturnsAsync(_workEnvironments.First(x => x.Id == testId));

            // Act
            var result = await _controller.GetWorkEnvironment(testId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedEnvironment = okResult.Value as WorkEnvironmentDto;

            Assert.That(returnedEnvironment, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedEnvironment.Id, Is.EqualTo(testId));
                Assert.That(returnedEnvironment.Name, Is.EqualTo($"Test {testId}"));
            });
        }

        [Test]
        public async Task GetWorkEnvironment_ReturnsNotFound_WhenEnvironmentDoesNotExist()
        {
            // Setup 
            var testId = 99;
            _mockService
                .Setup(service => service.ExistsAsync(testId))
                .ReturnsAsync(false);

            // Act 
            var result = await _controller.GetWorkEnvironment(999);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(errorResponse.Message, Is.EqualTo("WorkEnvironment not found"));
                Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }
    }
}
