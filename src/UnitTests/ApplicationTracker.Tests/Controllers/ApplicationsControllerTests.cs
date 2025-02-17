using ApplicationTracker.Common;
using ApplicationTracker.Controllers;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Enums;
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
            // Arrange
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
            // Arrange
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
            // Arrange
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
            // Arrange
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
            // Arrange
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
            // Arrange
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
            // Arrange
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
            // Arrange
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


        [Test]
        public async Task UpdateApplication_NullApplication_ReturnsBadRequest()
        {
            // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var result = await _controller.UpdateApplication(1, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                
                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Detail, Is.EqualTo("The application DTO cannot be null"));
            });
        }

        [Test]
        public async Task UpdateApplication_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var application = new ApplicationDto { Id = 2 };

            // Act
            var result = await _controller.UpdateApplication(1, application);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Detail, Is.EqualTo("The ID in the URL does not match the ID in the body"));

            });
        }

        [Test]
        public async Task UpdateApplication_InvalidApplication_ReturnsBadRequest()
        {
            // Arrange
            var application = new ApplicationDto { Id = 1 };


            // Act
            var result = await _controller.UpdateApplication(1, application);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result?.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid Application"));
            });
        }

        [Test]
        public async Task UpdateApplication_ApplicationNotFound_ReturnsNotFound()
        {
            // Arrange
            var application = new ApplicationDto 
            { 
                Id = 1, 
                Source = new SourceDto { Name = "Test" },
                Organization = new OrganizationDto { Name = "Test" },
                JobTitle = new JobTitleDto { Name = "Test" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Test" }
            };
            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateApplication(1, application);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
                
                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult?.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(notFoundResult?.Value, Is.EqualTo("Application with ID 1 not found."));
            });

        }

        [Test]
        public async Task UpdateApplication_ValidRequest_ReturnsOkWithUpdatedApplication()
        {
            // Arrange
            var application = new ApplicationDto
            {
                Id = 1,
                Source = new SourceDto { Name = "Test" },
                Organization = new OrganizationDto { Name = "Test" },
                JobTitle = new JobTitleDto { Name = "Test" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Test" }
            };
            var updatedApplication = new ApplicationDto
            {
                Id = 1,
                Source = new SourceDto { Name = "Test" },
                Organization = new OrganizationDto { Name = "Test" },
                JobTitle = new JobTitleDto { Name = "Test" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Test" }
            };

            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(true);
            _mockService.Setup(s => s.UpdateAsync(application)).ReturnsAsync(updatedApplication);

            // Act
            var result = await _controller.UpdateApplication(1, application);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult?.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(okResult?.Value, Is.EqualTo(updatedApplication));
            });
        }

        [Test]
        public async Task UpdateApplication_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var application = new ApplicationDto
            {
                Id = 1,
                Source = new SourceDto { Name = "Test" },
                Organization = new OrganizationDto { Name = "Test" },
                JobTitle = new JobTitleDto { Name = "Test" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Test" }
            };
            _mockService.Setup(s => s.ExistsAsync(1)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdateApplication(1, application);

            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<ObjectResult>());
                
                var objectResult = result.Result as ObjectResult;
                Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult?.Value, Is.TypeOf<ErrorResponse>());
               
                var errorResponse = objectResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("An unexpected error occurred while updating the application"));
            });
        }

        [Test]
        public async Task GetApplicationEvents_ReturnsBadRequest_WhenApplicationIdIsInvalid()
        {
            // Act
            var result = await _controller.GetApplicationEvents(0);
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid ID"));
            });
        }

        [Test]
        public async Task GetApplicationEvents_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(false);
            // Act
            var result = await _controller.GetApplicationEvents(1);
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult?.Value, Is.EqualTo("Application with ID 1 not found."));
            });
        }

        [Test]
        public async Task GetApplicationEvents_ReturnsOkResult_WhenEventsExist()
        {
            // Arrange
            var events = new List<AppEventDto>
            {
                new AppEventDto
                {
                    Id = 1, 
                    ApplicationId = 1, 
                    EventDate = DateTime.Now,  
                    ContactMethod = ContactMethod.Email.ToString(), 
                    EventType = EventType.Interview.ToString()
                },
                new AppEventDto
                {
                    Id = 2, 
                    ApplicationId = 1, 
                    EventDate = DateTime.Now.AddDays(-1), 
                    ContactMethod = ContactMethod.Email.ToString(), 
                    EventType = EventType.Interview.ToString()
                }
            };
            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(true);
            _mockService.Setup(s => s.GetEventsAsync(1)).ReturnsAsync(events);
            
            // Act
            var result = await _controller.GetApplicationEvents(1);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                
                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult?.Value, Is.EqualTo(events));
            });
        }

        [Test]
        public async Task GetApplicationEvents_RetrunsInternalServerError_OnException()
        {
            // Arrange
            var applictionId = 1;
            _mockService.Setup(s => s.ExistsAsync(applictionId)).ThrowsAsync(new Exception("Database error"));
            // Act
            var result = await _controller.GetApplicationEvents(1);
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<ObjectResult>());

                var objectResult = result.Result as ObjectResult;
                Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = objectResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo($"An unexpected error occurred while getting AppEvents for Application with Id {applictionId}"));
            });
        }


    }
}
