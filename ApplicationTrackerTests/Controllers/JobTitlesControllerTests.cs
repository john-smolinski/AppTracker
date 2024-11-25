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
    
    [TestFixture]
    public class JobTitlesControllerTests
    {
        private Mock<ILogger<JobTitlesController>> _mockLogger;
        private JobTitlesController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new TrackerDbContext(options);

            // seed data for tests
            context.JobTitles.AddRange(new List<JobTitle>()
            {
                new() { Id = 1, Name = "Test1" },
                new() { Id = 2, Name = "Test2" },
                new() { Id = 3, Name = "Test3" },
                new() { Id = 4, Name = "Test4" }
             });
            context.SaveChanges();

            _mockLogger = new Mock<ILogger<JobTitlesController>>();
            _controller = new JobTitlesController(context, _mockLogger.Object);
        }

        [Test]
        public async Task GetJobTitles_ReturnsOk_WhenJobTitlesExists()
        {
            // Act
            var result = await _controller.GetJobTitles();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);

            var returnedJobTitles = okResult.Value as IEnumerable<JobTitleDto>;
            Assert.That(returnedJobTitles, Is.Not.Null);
            Assert.That(returnedJobTitles.Count(), Is.EqualTo(4));
            Assert.That(returnedJobTitles.First().Name, Is.EqualTo("Test1"));
        }

        [Test]
        public async Task GetJobTitles_ReturnsNotFound_WhenNoJobTitlesExists()
        {
            // Setup
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var emptyContext = new TrackerDbContext(options);
            var controller = new JobTitlesController(emptyContext, _mockLogger.Object);

            // Act
            var result = await controller.GetJobTitles();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse.Message, Is.EqualTo("JobTitles not found"));
        }

        [Test]
        public async Task GetJobTitle_ReturnsOk_WhenJobTitleExists()
        {
            // Act 
            var result = await _controller.GetJobTitle(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedJobTitle = okResult.Value as JobTitleDto;

            Assert.That(returnedJobTitle, Is.Not.Null);
            Assert.That(returnedJobTitle.Id, Is.EqualTo(1));
            Assert.That(returnedJobTitle.Name, Is.EqualTo("Test1"));
        }

        [Test]
        public async Task GetJobTitle_ReturnsNotFound_WhenJobTitleDoesNotExist()
        {
            // Act 
            var result = await _controller.GetJobTitle(999); 

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse.Message, Is.EqualTo("JobTitle not found"));
            Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }
    }
}
