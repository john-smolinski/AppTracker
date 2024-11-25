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
    [TestFixture]
    public class WorkEnvironmentsControllerTests
    {
        private Mock<ILogger<WorkEnvironmentsController>> _mockLogger;
        private WorkEnvironmentsController _controller;

        [SetUp]
        public void SetUp()
        {
            // create an in memory context with 4 rows of test data 
            var context = TestContextHelper.GetInMemoryContext<WorkEnvironment>(4);

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
            Assert.That(returnedEnvironments.First().Name, Does.StartWith($"Test {typeof(WorkEnvironment).Name}"));
        }

        [Test]
        public async Task GetEnvironments_ReturnsNotFound_WhenNoEnvironmentsExist()
        {
            // Setup
            var emptyContext = TestContextHelper.GetInMemoryContext<WorkEnvironment>();
            var controller = new WorkEnvironmentsController(emptyContext, _mockLogger.Object);

            // Act
            var result = await controller.GetEnvironments();

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
            // Act
            var id = 1;
            var result = await _controller.GetWorkEnvironment(id);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedEnvironment = okResult.Value as WorkEnvironmentDto;

            Assert.That(returnedEnvironment, Is.Not.Null);
            Assert.That(returnedEnvironment.Id, Is.EqualTo(id));
            Assert.That(returnedEnvironment.Name, Is.EqualTo($"Test {typeof(WorkEnvironment).Name} {id}"));
        }

        [Test]
        public async Task GetWorkEnvironment_ReturnsNotFound_WhenEnvironmentDoesNotExist()
        {
            // Act 
            var result = await _controller.GetWorkEnvironment(999); 

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
