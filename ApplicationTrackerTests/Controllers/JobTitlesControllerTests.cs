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
using ApplicationTrackerTests.Helpers;

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
            // create an in memory context with 4 rows of test data 
            var context = ContextHelper.GetInMemoryContext<JobTitle>(4);
           
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
            Assert.Multiple(() =>
            {
                Assert.That(returnedJobTitles.Count(), Is.EqualTo(4));
                Assert.That(returnedJobTitles.First().Name, Does.StartWith($"Test {typeof(JobTitle).Name}"));
            });
        }

        [Test]
        public async Task GetJobTitles_ReturnsNotFound_WhenNoJobTitlesExists()
        {
            // Setup
            var emptyContext = ContextHelper.GetInMemoryContext<JobTitle>();
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
            var id = 1;
            var result = await _controller.GetJobTitle(id);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedJobTitle = okResult.Value as JobTitleDto;

            Assert.That(returnedJobTitle, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedJobTitle.Id, Is.EqualTo(id));
                Assert.That(returnedJobTitle.Name, Is.EqualTo($"Test {typeof(JobTitle).Name} {id}"));
            });
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
            Assert.Multiple(() => 
            { 
                Assert.That(errorResponse.Message, Is.EqualTo("JobTitle not found"));
                Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }
    }
}
