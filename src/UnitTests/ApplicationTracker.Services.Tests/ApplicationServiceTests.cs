using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Data.Enums;
using ApplicationTracker.TestUtilities.Helpers;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ApplicationTracker.Services.Tests
{
    [TestFixture]
    public class ApplicationServiceTests
    {
        private TrackerDbContext _context;
        private ILogger<ApplicationService> _logger;
        private ApplicationService _service;

        [SetUp]
        public void SetUp()
        {
            // add reject app event to context helper then test for reject 
            _context = ContextHelper.GetInMemoryContext<Application>(rows: 5);
            _logger = new LoggerFactory().CreateLogger<ApplicationService>();
            _service = new ApplicationService(_context, _logger);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllApplications()
        {
            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(5));
            });
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnApplication_WhenIdExists()
        {
            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Source.Id, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturneApplicationWithIsRejectedTrue_WhenApplicationIsRejected()
        {
            // Arrange
            ContextHelper.AddRejectionEvent(_context, 1);
            
            // Act
            var result = await _service.GetByIdAsync(1);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.IsRejected, Is.True);
            });
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturneApplicationWithIsRejectedFalse_WhenApplicationIsNotRejected()
        {
            // Act
            var result = await _service.GetByIdAsync(1);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.IsRejected, Is.False);
            });
        }

        [Test]
        public async Task PostAsync_ShouldAddNewApplication()
        {
            // Arrange
            var applicationDto = new ApplicationDto
            {
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now),
                Source = new SourceDto { Name = "Test Source 1" },
                Organization = new OrganizationDto { Name = "Test Organization 1" },
                JobTitle = new JobTitleDto {  Name = "Test JobTitle 1" },
                WorkEnvironment = new WorkEnvironmentDto {  Name = "Remote" },
            };

            // Act
            var result = await _service.PostAsync(applicationDto);

            // Assert 
            Assert.Multiple(() => 
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Source.Name, Is.EqualTo(applicationDto.Source.Name));
                Assert.That(result.Organization.Name, Is.EqualTo(applicationDto.Organization.Name));
                Assert.That(result.JobTitle.Name, Is.EqualTo(applicationDto.JobTitle.Name));
                Assert.That(result.WorkEnvironment.Name, Is.EqualTo(applicationDto.WorkEnvironment.Name));
            });
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrue_WhenApplicationExists()
        {
            // Act
            var result = await _service.ExistsAsync(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_WhenApplicationDoesNotExist()
        {
            // Act
            var result = await _service.ExistsAsync(999);

            // Assert
            Assert.That(result, Is.False);

        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrue_WhenApplicationWithDtoExists()
        {
            // Arrange
            var applicationDto = new ApplicationDto
            {
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                Source = new SourceDto { Name = "Test Source 1" },
                Organization = new OrganizationDto { Name = "Test Organization 1" },
                JobTitle = new JobTitleDto { Name = "Test JobTitle 1" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Test WorkEnvironment 1" }
            };

            // Act
            var result = await _service.ExistsAsync(applicationDto);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_WhenApplicationWithDtoDoesNotExist()
        {
            // Arrange
            var applicationDto = new ApplicationDto
            {
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now),
                Source = new SourceDto { Name = "Nonexistent Source" },
                Organization = new OrganizationDto { Name = "Nonexistent Organization" },
                JobTitle = new JobTitleDto { Name = "Nonexistent JobTitle" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Nonexistent WorkEnvironment" }
            };

            // Act
            var result = await _service.ExistsAsync(applicationDto);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public async Task UpdateAsync_ShouldThrowException_WhenApplicationIsInvalid()
        {
            foreach(var testCase in InvalidApplicationTestCases())
            {
                var applicationDto = (ApplicationDto)testCase.Arguments[0]!;
                var expectedExceptionType = (Type)testCase.Arguments[1]!;
                // Act
                Exception? exception = null;
                try
                {
                    await _service.UpdateAsync(applicationDto);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(exception, Is.Not.Null);
                    Assert.That(exception, Is.TypeOf(expectedExceptionType));
                });
            }
        }

        public static IEnumerable<TestCaseData> InvalidApplicationTestCases()
        {
            yield return new TestCaseData(
                new ApplicationDto { Id = null, Source = new SourceDto { Id = 1 }, Organization = new OrganizationDto { Id = 1 }, JobTitle = new JobTitleDto { Id = 1 }, WorkEnvironment = new WorkEnvironmentDto { Id = 1 } },
                typeof(InvalidOperationException)
            ).SetName("Application_Id_Is_Null");
            yield return new TestCaseData(
                new ApplicationDto { Id = 1, Source = new SourceDto { Id = null }, Organization = new OrganizationDto { Id = 1 }, JobTitle = new JobTitleDto { Id = 1 }, WorkEnvironment = new WorkEnvironmentDto { Id = 1 } },
                typeof(InvalidOperationException)
            ).SetName("Source_Id_Is_Null");
            yield return new TestCaseData(
                new ApplicationDto { Id = 1, Source = new SourceDto { Id = 1 }, Organization = new OrganizationDto { Id = null }, JobTitle = new JobTitleDto { Id = 1 }, WorkEnvironment = new WorkEnvironmentDto { Id = 1 } },
                typeof(InvalidOperationException)
            ).SetName("Organization_Id_Is_Null");
            yield return new TestCaseData(
                new ApplicationDto { Id = 1, Source = new SourceDto { Id = 1 }, Organization = new OrganizationDto { Id = 1 }, JobTitle = new JobTitleDto { Id = null }, WorkEnvironment = new WorkEnvironmentDto { Id = 1 } },
                typeof(InvalidOperationException)
            ).SetName("JobTitle_Id_Is_Null");
            yield return new TestCaseData(
                new ApplicationDto { Id = 1, Source = new SourceDto { Id = 1 }, Organization = new OrganizationDto { Id = 1 }, JobTitle = new JobTitleDto { Id = 1 }, WorkEnvironment = new WorkEnvironmentDto { Id = null } },
                typeof(InvalidOperationException)
            ).SetName("WorkEnvironment_Id_Is_Null");
        }

        [Test]
        public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenApplicationNotFound()
        {
            // Arrange
            var applicationDto = new ApplicationDto
            {
                Id = 999, // non-existent Id
                Source = new SourceDto { Id = 1 },
                Organization = new OrganizationDto { Id = 1 },
                JobTitle = new JobTitleDto { Id = 1 },
                WorkEnvironment = new WorkEnvironmentDto { Id = 1 }
            };

            // Act 
            var exception = await Task.Run(async () => {
                try
                {
                    await _service.UpdateAsync(applicationDto);
                    return null;
                }
                catch (KeyNotFoundException ex)
                {
                    return ex;
                }
            });

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception, Is.TypeOf<KeyNotFoundException>());
                Assert.That(exception?.Message, Is.EqualTo("Application with Id 999 not found."));
            });
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateApplication_WhenValidApplicationExists()
        {
            // Arrange
            var existingApplication = _context.Applications.First(); // Get an existing application
            var applicationDto = new ApplicationDto
            {
                Id = existingApplication.Id,
                ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Source = new SourceDto { Id = 1 },
                Organization = new OrganizationDto { Id = 1 },
                JobTitle = new JobTitleDto { Id = 1 },
                WorkEnvironment = new WorkEnvironmentDto { Id = 1 },
                City = "UpdatedCity",
                State = "UpdatedState"
            };

            // Act
            var result = await _service.UpdateAsync(applicationDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Id, Is.EqualTo(applicationDto.Id));
                Assert.That(result?.Source?.Id, Is.EqualTo(applicationDto.Source.Id));
                Assert.That(result?.Organization?.Id, Is.EqualTo(applicationDto.Organization.Id));
                Assert.That(result?.JobTitle?.Id, Is.EqualTo(applicationDto.JobTitle.Id));
                Assert.That(result?.WorkEnvironment?.Id, Is.EqualTo(applicationDto.WorkEnvironment.Id));
                Assert.That(result?.City, Is.EqualTo("UpdatedCity"));
                Assert.That(result?.State, Is.EqualTo("UpdatedState"));
            });
        }

        [Test]
        public async Task GetEventsAsync_ShouldReturnAllEvents()
        {
            // Arrange
            ContextHelper.AddAppEvents(_context, 1, 3);

            // Act
            var result = await _service.GetEventsAsync(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(3));
            });
        }

        [Test]
        public async Task GetEventsAsync_ShouldReturnEmptyList_WhenNoEventsExist()
        {
            // Act
            var result = await _service.GetEventsAsync(1);
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(0));
            });
        }


        [Test]
        public async Task GetEventByIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            ContextHelper.AddAppEvents(_context, 1, 1);
            // Act
            var result = await _service.GetEventByIdAsync(1, 1);
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Id, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GetEventByIdAsync_ShouldReturnNull_WhenEventDoesNotExist()
        {
            // Act
            var result = await _service.GetEventByIdAsync(1, 1);
            
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PostEventAsync_ShouldAddNewEvent()
        {
            // Arrange
            var applicationId = 1;

            var appEventDto = new AppEventDto
            {
                ApplicationId = applicationId,
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),           
                EventType = EventType.Interview.ToString(),
                Description = "Test Interview"

            };

            // Act
            var result = await _service.PostEventAsync(applicationId, appEventDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ApplicationId, Is.EqualTo(appEventDto.ApplicationId));
                Assert.That(result.EventDate, Is.EqualTo(appEventDto.EventDate));
                Assert.That(result.ContactMethod, Is.EqualTo(appEventDto.ContactMethod));
                Assert.That(result.EventType, Is.EqualTo(appEventDto.EventType));
                Assert.That(result.Description, Is.EqualTo(appEventDto.Description));
            });
        }

        [Test]
        public async Task PostEventAsync_ShouldThrowKeyNotFoundException_WhenApplicationNotFound()
        {
            // Arrange
            var appEventDto = new AppEventDto
            {
                ApplicationId = 999, // non-existent Id
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString(),
                Description = "Test Interview"
            };
            // Act
            var exception = await Task.Run(async () =>
            {
                try
                {
                    await _service.PostEventAsync(999, appEventDto);
                    return null;
                }
                catch (KeyNotFoundException ex)
                {
                    return ex;
                }
            });
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception, Is.TypeOf<KeyNotFoundException>());
                Assert.That(exception?.Message, Is.EqualTo("Application with Id 999 not found."));
            });
        }

        [Test]
        public async Task PostEventAsync_ShouldThrowInvalidEnumArgumentException_WhenContactMethodIsInvalid()
        {
            // Arrange
            var appEventDto = new AppEventDto
            {
                ApplicationId = 1,
                EventDate = DateTime.Now,
                ContactMethod = "InvalidContactMethod",
                EventType = EventType.Interview.ToString(),
                Description = "Test Interview"
            };
            // Act
            var exception = await Task.Run(async () =>
            {
                try
                {
                    await _service.PostEventAsync(1, appEventDto);
                    return null;
                }
                catch (InvalidEnumArgumentException ex)
                {
                    return ex;
                }
            });
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception, Is.TypeOf<InvalidEnumArgumentException>());
                Assert.That(exception?.Message, Is.EqualTo($"Invalid ContactMethod. Expected values: {string.Join(", ", Enum.GetNames(typeof(ContactMethod)))}"));
            });
        }

        [Test]
        public async Task PostEventAsync_ShouldThrowInvalidEnumArgumentException_WhenEventTypeIsInvalid()
        {
            // Arrange
            var appEventDto = new AppEventDto
            {
                ApplicationId = 1,
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = "InvalidEventType",
                Description = "Test Interview"
            };
            // Act
            var exception = await Task.Run(async () =>
            {
                try
                {
                    await _service.PostEventAsync(1, appEventDto);
                    return null;
                }
                catch (InvalidEnumArgumentException ex)
                {
                    return ex;
                }
            });
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception, Is.TypeOf<InvalidEnumArgumentException>());
                Assert.That(exception?.Message, Is.EqualTo($"Invalid EventType. Expected values: {string.Join(", ", Enum.GetNames(typeof(EventType)))}"));
            });
        }

        [Test]
        public async Task UpdateEventAsync_ShouldThrowException_WhenEventIsInvalid()
        {
            foreach (var testCase in InvalidAppEventTestCases())
            {
                var appEventDto = (AppEventDto)testCase.Arguments[0]!;
                var expectedExceptionType = (Type)testCase.Arguments[1]!;

                // Act
                Exception? exception = null;
                try
                {
                    await _service.UpdateEventAsync(1, appEventDto);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(exception, Is.Not.Null);
                    Assert.That(exception, Is.TypeOf(expectedExceptionType));
                });
            }
        }

        public static IEnumerable<TestCaseData> InvalidAppEventTestCases()
        {
            yield return new TestCaseData(
                new AppEventDto { Id = null, ApplicationId = 1, EventDate = DateTime.UtcNow },
                typeof(InvalidOperationException)
            ).SetName("Id_Is_Null");

            yield return new TestCaseData(
                new AppEventDto { Id = 1, ApplicationId = 99, EventDate = DateTime.UtcNow },
                typeof(InvalidOperationException)
            ).SetName("Event_Does_Not_Belong_To_Application");

            yield return new TestCaseData(
                new AppEventDto { Id = 1, ApplicationId = 1, EventDate = default },
                typeof(InvalidOperationException)
            ).SetName("EventDate_Is_Default");

            yield return new TestCaseData(
                new AppEventDto { Id = 1, ApplicationId = 1, EventDate = DateTime.UtcNow, ContactMethod = "InvalidMethod" , EventType="Email"},  // 🔴 Invalid Enum
                typeof(InvalidEnumArgumentException)
            ).SetName("Invalid_ContactMethod");

            yield return new TestCaseData(
                new AppEventDto { Id = 1, ApplicationId = 1, EventDate = DateTime.UtcNow, ContactMethod="Email", EventType = "InvalidMethod" },  // 🔴 Invalid Enum
                typeof(InvalidEnumArgumentException)
            ).SetName("Invalid_EventType");
        }

        [Test]
        public async Task UpdateEventAsync_ShouldThrowKeyNotFoundException_WhenAppEventNotFound()
        {
            // Arrange
            var appEventDto = new AppEventDto
            {
                Id = 999, // no app events exist
                ApplicationId = 1, 
                EventDate = DateTime.Now,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString(),
                Description = "Test Interview"
            };
            // Act
            var exception = await Task.Run(async () =>
            {
                try
                {
                    await _service.UpdateEventAsync(1, appEventDto);
                    return null;
                }
                catch (KeyNotFoundException ex)
                {
                    return ex;
                }
            });
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception, Is.TypeOf<KeyNotFoundException>());
                Assert.That(exception?.Message, Is.EqualTo("AppEvent with Id 999 not found."));
            });
        }

        [Test]
        public async Task UpdateEventAsync_ShouldUpdateAppEvent_WhenValidAppEventExists()
        {
            // Arrange
            ContextHelper.AddAppEvents(_context, 1, 1);

            var appEventDto = new AppEventDto
            {
                Id = 1,
                ApplicationId = 1,
                EventDate = DateTime.UtcNow,
                ContactMethod = ContactMethod.Email.ToString(),
                EventType = EventType.Interview.ToString(),
                Description = "Updated Interview"
            };
            // Act
            var result = await _service.UpdateEventAsync(1, appEventDto);
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Id, Is.EqualTo(appEventDto.Id));
                Assert.That(result?.ApplicationId, Is.EqualTo(appEventDto.ApplicationId));
                Assert.That(result?.EventDate, Is.EqualTo(appEventDto.EventDate));
                Assert.That(result?.ContactMethod, Is.EqualTo(appEventDto.ContactMethod));
                Assert.That(result?.EventType, Is.EqualTo(appEventDto.EventType));
                Assert.That(result?.Description, Is.EqualTo(appEventDto.Description));
            });
        }

        [Test]
        public async Task EventExistsAsync_ShouldReturnFalse_WhenAppEventDoesNotExist()
        {
            // Act
            var result = await _service.EventExistsAsync(1, 1);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EventExistsAsynce_ShouldReturnTrue_WhenAppEventExists()
        {
            // Arrange
            ContextHelper.AddAppEvents(_context, 1, 1);
            // Act
            var result = await _service.EventExistsAsync(1, 1);
            // Assert
            Assert.That(result, Is.True);
        }



        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
