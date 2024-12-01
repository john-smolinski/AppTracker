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
    public class JobTitlesControllerTests
    {
        private Mock<IService<JobTitleDto>> _mockService;
        private Mock<ILogger<JobTitlesController>> _mockLogger;
        private JobTitlesController _controller;

        private List<JobTitleDto> _jobTitles;
        private ServiceFactory _serviceFactory;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IService<JobTitleDto>>();
            _mockLogger = new Mock<ILogger<JobTitlesController>>();

            _jobTitles =
            [
                new() { Id = 1, Name = "Test 1" },
                new() { Id = 2, Name = "Test 2" },
                new() { Id = 3, Name = "Test 3" },
                new() { Id = 4, Name = "Test 4" }
            ];

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(JobTitleService)))
                .Returns(_mockService.Object);

            _serviceFactory = new ServiceFactory(mockServiceProvider.Object);
            _controller = new JobTitlesController(_serviceFactory, _mockLogger.Object);
        }

        [Test]
        public async Task GetJobTitles_ReturnsOk_WhenJobTitlesExists()
        {
            // Setup
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync(_jobTitles);

            // Act
            var result = await _controller.GetJobTitles();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
                
                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult, Is.Not.Null);

                var returnedJobTitles = okResult!.Value as IEnumerable<JobTitleDto>;
                Assert.That(returnedJobTitles, Is.Not.Null);
                Assert.That(returnedJobTitles!.Count(), Is.EqualTo(4));
                Assert.That(returnedJobTitles!.First().Name, Does.StartWith("Test"));
            });
        }

        [Test]
        public async Task GetJobTitles_ReturnsNotFound_WhenNoJobTitlesExists()
        {
            // Setup
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync(new List<JobTitleDto>());

            // Act
            var result = await _controller.GetJobTitles();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());

                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult, Is.Not.Null);
                
                var errorResponse = notFoundResult!.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse!.Message, Is.EqualTo("JobTitles not found"));
            });
        }

        [Test]
        public async Task GetJobTitle_ReturnsOk_WhenJobTitleExists()
        {
            // Setup
            var testId = 1;
            
            _mockService
                .Setup(service => service.ExistsAsync(testId))
                .ReturnsAsync(true);
            
            _mockService
                .Setup(service => service.GetByIdAsync(testId))
                 .ReturnsAsync(_jobTitles.First(x => x.Id == testId));

            // Act
            var result = await _controller.GetJobTitle(testId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
                
                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult, Is.Not.Null);

                var returnedJobTitle = okResult!.Value as JobTitleDto;
                Assert.That(returnedJobTitle, Is.Not.Null);
                Assert.That(returnedJobTitle!.Id, Is.EqualTo(testId));
                Assert.That(returnedJobTitle.Name, Is.EqualTo($"Test {testId}"));
            });
        }

        [Test]
        public async Task GetJobTitle_ReturnsNotFound_WhenJobTitleDoesNotExist()
        {
            // Setup
            var testId = 99;
            _mockService
                .Setup(service => service.ExistsAsync(testId))
                .ReturnsAsync(false);

            // Act 
            var result = await _controller.GetJobTitle(testId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
                
                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult, Is.Not.Null);

                var errorResponse = notFoundResult!.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse!.Message, Is.EqualTo("JobTitle not found"));
                Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }
    }
}
