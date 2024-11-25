using ApplicationTracker.Common;
using ApplicationTracker.Controllers;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
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
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new TrackerDbContext(options);

            // seed data for tests
            context.Locations.AddRange(new List<Location>
            {
                new() { Id = 1, City = "Test1", State="Test1" },
                new() { Id = 2, City = "Test2", State="Test1" },
                new() { Id = 3, City = "Test3", State="Test1" },
                new() { Id = 4, City = "Test4", State="Test1" }
            });
            context.SaveChanges();

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
            Assert.That(returnedLocations.Count(), Is.EqualTo(4));
            Assert.That(returnedLocations.First().City, Is.EqualTo("Test1"));
        }

        [Test]
        public async Task GetLocations_ReturnsNotFound_WhenNoLocationsExists()
        {
            // Setup
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var emptyContext = new TrackerDbContext(options);
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
            var result = await _controller.GetLocation(1);

            // Asert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedLocation = okResult.Value as LocationDto;

            Assert.That(returnedLocation, Is.Not.Null);
            Assert.That(returnedLocation.Id, Is.EqualTo(1));
            Assert.That(returnedLocation.City, Is.EqualTo("Test1"));
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
            Assert.That(errorResponse.Message, Is.EqualTo("Location not found"));
            Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }
    }
}
