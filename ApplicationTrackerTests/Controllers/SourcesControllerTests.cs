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
    public class SourcesControllerTests
    {
        private Mock<ILogger<SourcesController>> _mockLogger;
        private SourcesController _controller;

        [SetUp]
        public void Setup()
        {
            // create an in memory context with 4 rows of test data 
            var context = ContextHelper.GetInMemoryContext<Source>(4);

            _mockLogger = new Mock<ILogger<SourcesController>>();
            _controller = new SourcesController(context, _mockLogger.Object);
        }

        [Test]
        public async Task GetSources_ReturnsOk_WhenSourcesExists()
        {
            // Act
            var result = await _controller.GetSources();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);

            var returnedSources = okResult.Value as IEnumerable<SourceDto>;
            Assert.That(returnedSources, Is.Not.Null);
            Assert.That(returnedSources.Count(), Is.EqualTo(4));
            Assert.That(returnedSources.First().Name, Does.StartWith($"Test {typeof(Source).Name}"));
        }

        [Test]
        public async Task GetSources_ReturnsNotFound_WhenNoSourcesExists()
        {
            // Setup
            var emptyContext = ContextHelper.GetInMemoryContext<Source>();
            var controller = new SourcesController(emptyContext, _mockLogger.Object);

            // Act
            var result = await controller.GetSources();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse.Message, Is.EqualTo("Sources missing"));
        }

        [Test]
        public async Task GetSource_ReturnsOk_WhenEnvironmentExists()
        {
            // Act
            var id = 1;
            var result = await _controller.GetSource(id);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedSource = okResult.Value as SourceDto;

            Assert.That(returnedSource, Is.Not.Null);
            Assert.That(returnedSource.Id, Is.EqualTo(id));
            Assert.That(returnedSource.Name, Is.EqualTo($"Test {typeof(Source).Name} {id}"));
        }

        [Test]
        public async Task GetSource_ReturnsNotFound_WhenNoSourceExist()
        {
            // Act
            var result = await _controller.GetSource(999);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse.Message, Is.EqualTo("Source not found"));
            Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }
    }
}
