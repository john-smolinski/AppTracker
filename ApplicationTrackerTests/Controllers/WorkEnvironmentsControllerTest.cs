using ApplicationTracker.Controllers;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using Microsoft.AspNetCore.Mvc;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Common;
using Microsoft.AspNetCore.Http;

namespace ApplicationTrackerTests.Controllers
{
    [TestFixture]
    public class WorkEnvironmentsControllerTests
    {
        private Mock<ILogger<WorkEnvironmentsController>> _mockLogger;
        private WorkEnvironmentsController _controller;

        [SetUp]
        public void SetUp()
        {
            // using an in memory database for unit tests
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new TrackerDbContext(options);

            // same seed data for most tests
            context.WorkEnvironments.AddRange(new List<WorkEnvironment>
            {
                new WorkEnvironment { Id = 1, Name = "Unknown" },
                new WorkEnvironment { Id = 2, Name = "On-Site" },
                new WorkEnvironment { Id = 3, Name = "Hybrid" },
                new WorkEnvironment { Id = 4, Name = "Remote" }
            });
            context.SaveChanges();

            // initiale mocks
            _mockLogger = new Mock<ILogger<WorkEnvironmentsController>>();
            _controller = new WorkEnvironmentsController(context, _mockLogger.Object);
        }

        [Test]
        public async Task GetEnvironments_ReturnsOk_WhenEnvironmentsExist()
        {
            // Act
            var result = await _controller.GetEnvironments();
            
            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            
            Assert.That(okResult, Is.Not.Null);

            var returnedEnvironments = okResult.Value as IEnumerable<WorkEnvironmentDto>;
            Assert.That(returnedEnvironments, Is.Not.Null);
            Assert.That(returnedEnvironments.Count(), Is.EqualTo(4));
            Assert.That(returnedEnvironments.First().Name, Is.EqualTo("Unknown"));
        }

        [Test]
        public async Task GetEnvironments_ReturnsNotFound_WhenNoEnvironmentsExist()
        {
            // Setup
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var emptyContext = new TrackerDbContext(options);
            var controller = new WorkEnvironmentsController(emptyContext, _mockLogger.Object);

            // Act
            var result = await controller.GetEnvironments();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null); 
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null); 
            // verify error message returned by controller
            Assert.That(errorResponse.Message, Is.EqualTo("WorkEnvironments empty or missing")); 
        }

        [Test]
        public async Task GetWorkEnvironment_ReturnsOk_WhenEnvironmentExists()
        {
            // Act
            var result = await _controller.GetWorkEnvironment(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedEnvironment = okResult.Value as WorkEnvironmentDto;

            Assert.That(returnedEnvironment, Is.Not.Null);
            Assert.That(returnedEnvironment.Id, Is.EqualTo(1));
            Assert.That(returnedEnvironment.Name, Is.EqualTo("Unknown"));
        }

        [Test]
        public async Task GetWorkEnvironment_ReturnsNotFound_WhenEnvironmentDoesNotExist()
        {
            // Act 
            var result = await _controller.GetWorkEnvironment(999); // non existant value

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse.Message, Is.EqualTo("WorkEnvironment not found"));
            Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }
    }
}
