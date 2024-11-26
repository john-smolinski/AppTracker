using ApplicationTracker.Common;
using ApplicationTracker.Controllers;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTrackerTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;

namespace ApplicationTrackerTests.Controllers
{
    public class LocationsControllerTests
    {
        private Mock<ILogger<LocationsController>> _mockLogger;
        private LocationsController _controller;

        [SetUp]
        public void Setup()
        {
            var context = ContextHelper.GetInMemoryContext<Location>(4);

            _mockLogger = new Mock<ILogger<LocationsController>>();
            _controller = new LocationsController(context, _mockLogger.Object);
        }

        [Test]
        public async Task GetLocations_ReturnsOk_WhenLocationsExists()
        {
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
                Assert.That(returnedLocations.First().Name, Does.StartWith($"Test {typeof(Location).Name}"));
            });
        }

        [Test]
        public async Task GetLocations_ReturnsNotFound_WhenNoLocationsExists()
        {
            // Setup
            var emptyContext = ContextHelper.GetInMemoryContext<Location>();
            var controller = new LocationsController(emptyContext, _mockLogger.Object);

            // Act 
            var result = await controller.GetLocations();

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
            // Act 
            var id = 1;
            var result = await _controller.GetLocation(id);

            // Asert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedLocation = okResult.Value as LocationDto;

            Assert.That(returnedLocation, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedLocation.Id, Is.EqualTo(id));
                Assert.That(returnedLocation.Name, Is.EqualTo($"Test {typeof(Location).Name} {id}"));
            });
        }

        [Test]
        public async Task GetLocation_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            // Act 
            var result = await _controller.GetLocation(999); 

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
