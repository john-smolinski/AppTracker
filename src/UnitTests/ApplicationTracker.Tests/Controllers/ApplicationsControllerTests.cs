using ApplicationTracker.Common;
using ApplicationTracker.Controllers;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApplicationTracker.Tests.Controllers
{
    public class ApplicationsControllerTests
    {
        private Mock<IApplicationService<ApplicationDto>> _mockService;
        private Mock<ILogger<ApplicationsController>> _mockLogger;
        private ApplicationsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IApplicationService<ApplicationDto>>();
            _mockLogger = new Mock<ILogger<ApplicationsController>>();
            _controller = new ApplicationsController(_mockService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllApplications_ReturnsOkResult_WhenApplicationsExist()
        {
            // Setup
            var applications = new List<ApplicationDto>
            {
                new ApplicationDto { Id = 1, ApplicationDate = DateOnly.FromDateTime(DateTime.Now) },
                new ApplicationDto { Id = 2, ApplicationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)) }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(applications);

            // Act
            var result = await _controller.GetAllApplications();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                
                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult?.Value, Is.EqualTo(applications));
            });
        }

        [Test]
        public async Task GetAllApplications_ReturnsNotFound_WhenNoApplicationsExist()
        {
            // Setup
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApplicationDto>());

            // Act
            var result = await _controller.GetAllApplications();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
                
                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult?.Value, Is.TypeOf<ErrorResponse>());
                
                var errorResponse = notFoundResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Applications not found"));
            });
        }

        [Test]
        public async Task GetApplication_ReturnsOkResult_WhenApplicationExists()
        {
            // Setup
            var application = new ApplicationDto { Id = 1, ApplicationDate = DateOnly.FromDateTime(DateTime.Now) };
            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(true);
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(application);

            // Act
            var result = await _controller.GetApplication(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                
                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult?.Value, Is.EqualTo(application));
            });
        }

        [Test]
        public async Task GetApplication_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            // Setup
            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.GetApplication(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
               
                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult?.Value, Is.TypeOf<ErrorResponse>());
                
                var errorResponse = notFoundResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Application not found"));
            });
        }

        [Test]
        public async Task PostNewApplication_ReturnsOkResult_WhenApplicationIsValid()
        {
            // Setup
            var application = new ApplicationDto
            {
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now),
                Source = new SourceDto{ Name = "Test Source" },
                Organization = new OrganizationDto{ Name = "Test Organization" },
                JobTitle = new JobTitleDto{ Name = "Test JobTitle" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Test WorkEnvironment" }
            };

            _mockService.Setup(s => s.PostAsync(application)).ReturnsAsync(application);

            // Act
            var result = await _controller.PostNewApplication(application);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
                
                var createdAtActionResult = result.Result as CreatedAtActionResult;
                Assert.That(createdAtActionResult?.Value, Is.EqualTo(application));
            });
        }

        [Test]
        public async Task PostNewApplication_Returns_Conflict_WhenApplicationAlreadyExists()
        {
            // Setup
            var application = new ApplicationDto
            {
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now),
                Source = new SourceDto { Name = "Test Source" },
                Organization = new OrganizationDto { Name = "Test Organization" },
                JobTitle = new JobTitleDto { Name = "Test JobTitle" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Test WorkEnvironment" }
            };
            
            _mockService.Setup(s => s.ExistsAsync(application)).ReturnsAsync(true);

            // Act 
            var result = await _controller.PostNewApplication(application);

            // Assert
            Assert.That(result.Result, Is.TypeOf<ConflictResult>());
        }

        [Test]
        public async Task PostNewApplication_ReturnsBadRequest_WhenApplicationIsInvalid()
        {
            // Setup
            _mockService.Setup(s => s.PostAsync(It.IsAny<ApplicationDto>()))
                        .ThrowsAsync(new ArgumentException("Invalid application"));

            var invalidApplication = new ApplicationDto
            {
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now)
            };

            // Act
            var result = await _controller.PostNewApplication(invalidApplication);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
                
                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());
                
                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid Application"));
            });
        }

        [Test]
        public async Task PostNewApplication_ReturnsInternalServerError_OnException()
        {
            // Setup
            var application = new ApplicationDto 
            { 
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now),
                Source = new SourceDto { Name = "Test Source" },
                Organization = new OrganizationDto { Name = "Test Organization" },
                JobTitle = new JobTitleDto { Name = "Test JobTitle" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Test WorkEnvironment" }
            };
            _mockService.Setup(s => s.PostAsync(application)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.PostNewApplication(application);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<ObjectResult>());
                
                var objectResult = result.Result as ObjectResult;
                Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult?.Value, Is.TypeOf<ErrorResponse>());
                
                var errorResponse = objectResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("An unexpected errror occured while posting new Application"));
            });
        }
    }
}
