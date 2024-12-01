using ApplicationTracker.Common;
using ApplicationTracker.Controllers;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services;
using ApplicationTracker.Services.Factory;
using ApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;

namespace ApplicationTrackerTests.Controllers
{
    [TestFixture]
    public class SourcesControllerTests
    {
        private Mock<IService<SourceDto>> _mockService;
        private Mock<ILogger<SourcesController>> _mockLogger;
        private SourcesController _controller;

        private List<SourceDto> _sources;
        private ServiceFactory _serviceFactory;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IService<SourceDto>>();
            _mockLogger = new Mock<ILogger<SourcesController>>();

            _sources =
            [
                new() { Id = 1, Name = "Test 1" },
                new() { Id = 2, Name = "Test 2" },
                new() { Id = 3, Name = "Test 3" },
                new() { Id = 4, Name = "Test 4" }
            ];

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(SourceService)))
                .Returns(_mockService.Object);  

            _serviceFactory = new ServiceFactory(mockServiceProvider.Object);
            _controller = new SourcesController(_serviceFactory, _mockLogger.Object);
        }

        [Test]
        public async Task GetSources_ReturnsOk_WhenSourcesExists()
        {
            // Setup 
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync(_sources);

            // Act
            var result = await _controller.GetSources();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            
                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult, Is.Not.Null);

                var returnedSources = okResult!.Value as IEnumerable<SourceDto>;
                Assert.That(returnedSources, Is.Not.Null);
                Assert.That(returnedSources!.Count(), Is.EqualTo(4));
                Assert.That(returnedSources!.First().Name, Does.StartWith($"Test"));
            });
        }

        [Test]
        public async Task GetSources_ReturnsNotFound_WhenNoSourcesExists()
        {
            // Setup
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync(new List<SourceDto>());

            // Act
            var result = await _controller.GetSources();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
                
                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult, Is.Not.Null);

                var errorResponse = notFoundResult!.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse!.Message, Is.EqualTo("Sources missing"));
            });
        }

        [Test]
        public async Task GetSource_ReturnsOk_WhenEnvironmentExists()
        {
            // Setup 
            var testId = 1;

            _mockService
                .Setup(service => service.ExistsAsync(testId))
                .ReturnsAsync(true);

            _mockService
                .Setup(service => service.GetByIdAsync(testId))
                .ReturnsAsync(_sources.First(x => x.Id == testId));


            // Act
            var result = await _controller.GetSource(testId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult, Is.Not.Null);

                var returnedSource = okResult!.Value as SourceDto;
                Assert.That(returnedSource, Is.Not.Null);

                Assert.That(returnedSource!.Id, Is.EqualTo(testId));
                Assert.That(returnedSource.Name, Is.EqualTo($"Test {testId}"));
            });
        }

        [Test]
        public async Task GetSource_ReturnsNotFound_WhenNoSourceExist()
        {
            // Setup 
            var testId = 99;
            _mockService
                .Setup(service => service.ExistsAsync (testId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.GetSource(testId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
                
                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult, Is.Not.Null);

                var errorResponse = notFoundResult!.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse!.Message, Is.EqualTo("Source not found"));
                Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }
    }
}
