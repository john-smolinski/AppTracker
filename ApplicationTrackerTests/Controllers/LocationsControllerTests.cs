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
    public class LocationsControllerTests
    {
        private Mock<IService<LocationDto>> _mockService;
        private Mock<ILogger<LocationsController>> _mockLogger;
        private LocationsController _controller;

        private List<LocationDto> _locations;
        private ServiceFactory _serviceFactory;


        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IService<LocationDto>>();
            _mockLogger = new Mock<ILogger<LocationsController>>();

            _locations =
            [
                new() { Id = 1, Name = "Test 1" },
                new() { Id = 2, Name = "Test 2" },
                new() { Id = 3, Name = "Test 3" },
                new() { Id = 4, Name = "Test 4" }
            ];

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(sp =>  sp.GetService(typeof(LocationService)))
                .Returns(_mockService.Object);

            _serviceFactory = new ServiceFactory(mockServiceProvider.Object);
            _controller = new LocationsController(_serviceFactory, _mockLogger.Object);
        }

        [Test]
        public async Task GetLocations_ReturnsOk_WhenLocationsExists()
        {
            // Setup
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync(_locations);

            // Act
            var result = await _controller.GetLocations();

            // Assert
            Assert.That(result.Result, Is.Not.Null);
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);

            var returnedLocations = okResult.Value as IEnumerable<LocationDto>;
            Assert.That(returnedLocations, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedLocations.Count(), Is.EqualTo(4));
                Assert.That(returnedLocations.First().Name, Does.StartWith($"Test"));
            });
        }

        [Test]
        public async Task GetLocations_ReturnsNotFound_WhenNoLocationsExists()
        {
            // Setup
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync([]);

            // Act 
            var result = await _controller.GetLocations();

            // Asert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse.Message, Is.EqualTo("Locations not found"));
        }

        [Test]
        public async Task GetLocation_ReturnsOk_WhenLocationExists()
        {
            // Setup 
            var testId = 1;

            _mockService
                .Setup(service => service.ExistsAsync(testId))
                .ReturnsAsync(true);

            _mockService
                .Setup(service => service.GetByIdAsync(testId))
                .ReturnsAsync(_locations.First(x => x.Id == testId));

            // Act 
            var result = await _controller.GetLocation(testId);

            // Asert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedLocation = okResult.Value as LocationDto;

            Assert.That(returnedLocation, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedLocation.Id, Is.EqualTo(testId));
                Assert.That(returnedLocation.Name, Is.EqualTo($"Test {testId}"));
            });
        }

        [Test]
        public async Task GetLocation_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            // Setup 
            var testId = 99;
            _mockService
                .Setup(service => service.ExistsAsync(testId))
                .ReturnsAsync(false);

            // Act 
            var result = await _controller.GetLocation(testId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(errorResponse.Message, Is.EqualTo("Location not found"));
                Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }
    }
}
