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

            _mockService.Verify(s => s.GetAllAsync(), Times.Once);
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

            _mockService.Verify(s => s.GetAllAsync(), Times.Once);
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

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
            _mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
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

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
        }

        [Test]
        public async Task PostNewApplication_ReturnsOkResult_WhenApplicationIsValid()
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

            _mockService.Verify(s => s.PostAsync(application), Times.Once);
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

            _mockService.Verify(s => s.ExistsAsync(application), Times.Once);
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

            _mockService.Verify(s => s.PostAsync(application), Times.Once);
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
                Assert.That(errorResponse?.Detail, Is.EqualTo("The Id in the URL does not match the Id in the body"));

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
                Assert.That(notFoundResult?.Value, Is.EqualTo("Application with Id 1 not found."));
            });

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
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

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
            _mockService.Verify(s => s.UpdateAsync(application), Times.Once);
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

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
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
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid Id"));
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
                Assert.That(notFoundResult?.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(notFoundResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = notFoundResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Application not found"));
            });

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
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

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
            _mockService.Verify(s => s.GetEventsAsync(1), Times.Once);
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

            _mockService.Verify(s => s.ExistsAsync(applictionId), Times.Once);
        }

        [Test]
        public async Task GetAppByEventById_ReturnsBadRequest_WhenApplicationIdIsInvalid()
        {
            // Act
            var result = await _controller.GetAppEventById(0, 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid Id"));
            });
        }

        [Test]
        public async Task GetAppEventById_ReturnsBadRequest_WhentEventIdIsInvalid()
        {
            // Act
            var result = await _controller.GetAppEventById(1, 0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid Id"));
            });
        }

        [Test]
        public async Task GetAppEventById_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.GetAppEventById(1, 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());

                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = notFoundResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Application not found"));
            });

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
        }

        [Test]
        public async Task GetAppEventById_ReturnsNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(true);
            _mockService.Setup(s => s.EventExistsAsync(1, 1)).ReturnsAsync(false);

            // Act
            var result = await _controller.GetAppEventById(1, 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());

                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = notFoundResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Event not found"));
            });

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
            _mockService.Verify(s => s.EventExistsAsync(1, 1), Times.Once);
        }

        [Test]
        public async Task GetAppEventById_ReturnsOkResult_WhenEventExists()
        {
            // Arrange
            var appEvent = new AppEventDto
            {
                Id = 1,
                ApplicationId = 1,
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString()
            };
            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(true);
            _mockService.Setup(s => s.EventExistsAsync(1, 1)).ReturnsAsync(true);
            _mockService.Setup(s => s.GetEventByIdAsync(1, 1)).ReturnsAsync(appEvent);

            // Act
            var result = await _controller.GetAppEventById(1, 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult?.Value, Is.EqualTo(appEvent));
            });

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
            _mockService.Verify(s => s.EventExistsAsync(1, 1), Times.Once);
            _mockService.Verify(s => s.GetEventByIdAsync(1, 1), Times.Once);
        }

        [Test]
        public async Task GetAppEventById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var applicationId = 1;
            var eventId = 1;
            _mockService.Setup(s => s.ExistsAsync(applicationId)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAppEventById(applicationId, eventId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<ObjectResult>());

                var objectResult = result.Result as ObjectResult;
                Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = objectResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo($"An unexpected error occurred while getting AppEvent with Id {eventId} for Application with Id {applicationId}"));
            });

            _mockService.Verify(s => s.ExistsAsync(applicationId), Times.Once);
        }

        [Test]
        public async Task PostAppEvent_ReturnsBadRequest_WhenAppEventDtoIsNull()
        {
            // Act
#pragma warning disable CS8625 
            var result = await _controller.PostAppEvent(1, null);
#pragma warning restore CS8625 

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Detail, Is.EqualTo("The AppEvent DTO cannot be null"));
            });

        }

        [Test]
        public async Task PostAppEvent_ReturnsBadRequest_WhenApplicationIdIsInvalid()
        {
            // Act
            var result = await _controller.PostAppEvent(0, new AppEventDto());
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid Id"));
            });
        }

        [Test]
        public async Task PostAppEvent_ReturnsBadRequest_WhenAppEventIsInvalid()
        {
            // Arrange
            // invalid AppEventDto
            var appEvent = new AppEventDto
            {
                ApplicationId = 1,
                EventDate = default,
                ContactMethod = "Invalid",
                EventType = "Invalid"
            };

            // Act
            var result = await _controller.PostAppEvent(1, appEvent);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid AppEvent"));
            });
        }

        [Test]
        public async Task PostAppEvent_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            // Arrange
            var appEvent = new AppEventDto
            {
                ApplicationId = 1,
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString()
            };
            _mockService.Setup(s => s.ExistsAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.PostAppEvent(1, appEvent);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());

                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = notFoundResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Application not found"));
            });

            _mockService.Verify(s => s.ExistsAsync(1), Times.Once);
        }

        [Test]
        public async Task PostAppEvent_ShouldReturnCreatedAtAction_WhenAppEventIsValid()
        {
            // Arrange
            int applicationId = 1;
            var appEventDto = new AppEventDto
            {
                ApplicationId = applicationId,
                EventDate = DateTime.UtcNow,
                EventType = EventType.Interview.ToString(),
                ContactMethod = ContactMethod.Email.ToString(),
                Description = "Successful event"
            };

            var savedEventDto = new AppEventDto
            {
                Id = 100, // Simulating an assigned Id after save
                ApplicationId = applicationId,
                EventDate = appEventDto.EventDate,
                EventType = appEventDto.EventType,
                ContactMethod = appEventDto.ContactMethod,
                Description = appEventDto.Description
            };

            _mockService.Setup(s => s.ExistsAsync(applicationId)).ReturnsAsync(true);

            _mockService.Setup(s => s.PostEventAsync(applicationId, appEventDto)).ReturnsAsync(savedEventDto);

            // Act
            var result = await _controller.PostAppEvent(applicationId, appEventDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());

                var createdResult = result.Result as CreatedAtActionResult;
                Assert.That(createdResult?.Value, Is.EqualTo(savedEventDto));
                Assert.That(createdResult?.ActionName, Is.EqualTo(nameof(_controller.GetAppEventById)));
                Assert.That(createdResult?.RouteValues?["applicationId"], Is.EqualTo(applicationId));
                Assert.That(createdResult?.RouteValues?["eventId"], Is.EqualTo(savedEventDto.Id));
            });

            // Verify that the service methods were called
            _mockService.Verify(s => s.ExistsAsync(applicationId), Times.Once);
            _mockService.Verify(s => s.PostEventAsync(applicationId, appEventDto));
        }

        [Test]
        public async Task PostAppEvent_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            int applicationId = 1;
            var appEventDto = new AppEventDto
            {
                ApplicationId = applicationId,
                EventDate = DateTime.UtcNow,
                EventType = EventType.Interview.ToString(),
                ContactMethod = ContactMethod.Email.ToString(),
                Description = "Successful event"
            };

            _mockService.Setup(s => s.ExistsAsync(applicationId)).ReturnsAsync(true);
            _mockService.Setup(s => s.PostEventAsync(applicationId, appEventDto)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.PostAppEvent(applicationId, appEventDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

                var objectResult = result.Result as ObjectResult;
                Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = objectResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo($"An unexpected error occurred while posting new AppEvent for Application with Id {applicationId}"));
            });
            _mockService.Verify(s => s.ExistsAsync(applicationId), Times.Once);
            _mockService.Verify(s => s.PostEventAsync(applicationId, appEventDto), Times.Once);
        }

        [Test]
        public async Task UpdateAppEvent_ReturnsBadRequest_WhenAppEventDtoIsNull()
        {
            // Act
#pragma warning disable CS8625 
            var result = await _controller.UpdateAppEvent(1, 1, null);
#pragma warning restore CS8625 
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Detail, Is.EqualTo("The AppEvent DTO cannot be null"));
            });
        }

        [Test]
        public async Task UpdateAppEvent_ReturnsBadRequest_WhenApplicationIdIsInvalid()
        {
            // Act
            var result = await _controller.UpdateAppEvent(0, 1, new AppEventDto());
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());
                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid Id"));
            });
        }

        [Test]
        public async Task UpdateAppEvent_ReturnsBadRequest_WhenAppEventIsInvalid()
        {
            // Arrange
            var invalidAppEvent = new AppEventDto
            {
                ApplicationId = 1,
                EventDate = default,
                ContactMethod = "Invalid",
                EventType = "Invalid"
            };

            // Act
            var result = await _controller.UpdateAppEvent(1, 1, invalidAppEvent);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = badRequestResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("Invalid AppEvent"));
            });
        }

        [Test]
        [TestCase(null)]
        [TestCase(999)]
        public async Task UpdateAppEvent_ReturnsBadRequest_WhenEventIdIsInvalid(int? eventId)
        {
            // Arrange
            var appEvent = new AppEventDto
            {
                Id = eventId,
                ApplicationId = 1,
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString()
            };

            // Act
            var result = await _controller.UpdateAppEvent(1, 1, new AppEventDto());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

                var badRequestResult = result.Result as BadRequestObjectResult;
                Assert.That(badRequestResult?.Value, Is.TypeOf<ErrorResponse>());
            });
        }

        [Test]
        public async Task UpdateAppEvent_ReturnsNotFound_WhenAppEventDoesntExist()
        {
            // Arrange
            var appEvent = new AppEventDto
            {
                Id = 1,
                ApplicationId = 1,
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString()
            };

            _mockService.Setup(s => s.EventExistsAsync(1, 1)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateAppEvent(1, 1, appEvent);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
                var notFoundResult = result.Result as NotFoundObjectResult;
                Assert.That(notFoundResult?.Value, Is.TypeOf<ErrorResponse>());

                var errorResponse = notFoundResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo("AppEvent not found"));
            });

            _mockService.Verify(s => s.EventExistsAsync(1, 1), Times.Once);
        }

        [Test]
        public async Task UpdateAppEvent_ReturnsOkayObjectResult_WhenAppEventIsUpdated()
        {
            // Arrange
            var applicationId = 1;

            var appEvent = new AppEventDto
            {
                Id = 1,
                ApplicationId = 1,
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString()
            };

            var updatedAppEvent = new AppEventDto
            {
                Id = 1,
                ApplicationId = 1,
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString()
            };

            _mockService.Setup(s => s.EventExistsAsync(1, 1)).ReturnsAsync(true);
            _mockService.Setup(s => s.UpdateEventAsync(applicationId, appEvent)).ReturnsAsync(updatedAppEvent);

            // Act
            var result = await _controller.UpdateAppEvent(applicationId, 1, appEvent);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                var okResult = result.Result as OkObjectResult;
                Assert.That(okResult?.Value, Is.EqualTo(updatedAppEvent));
            });

            _mockService.Verify(s => s.EventExistsAsync(1, 1), Times.Once);
            _mockService.Verify(s => s.UpdateEventAsync(applicationId, appEvent), Times.Once);
        }

        [Test]
        public async Task UpdateAppEvent_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var applicationId = 1;
            var appEvent = new AppEventDto
            {
                Id = 1,
                ApplicationId = 1,
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString()
            };
            
            _mockService.Setup(s => s.EventExistsAsync(1, 1)).ThrowsAsync(new Exception("Database error"));
            // Act
            var result = await _controller.UpdateAppEvent(applicationId, 1, appEvent);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<ObjectResult>());
                var objectResult = result.Result as ObjectResult;
                Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(objectResult?.Value, Is.TypeOf<ErrorResponse>());
                
                var errorResponse = objectResult?.Value as ErrorResponse;
                Assert.That(errorResponse?.Message, Is.EqualTo($"An unexpected error occurred while updating AppEvent with Id {appEvent.Id} for Application with Id {applicationId}"));
            });
            _mockService.Verify(s => s.EventExistsAsync(1, 1), Times.Once);
        }
    }
}