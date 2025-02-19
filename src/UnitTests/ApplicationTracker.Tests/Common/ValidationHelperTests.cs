using ApplicationTracker.Common;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace ApplicationTracker.Tests.Common
{
    [TestFixture]
    public class ValidationHelperTests
    {
        [Test]
        public void IsValidId_ReturnsTrue_WhenIdIsGreaterThanZero()
        {
            // Arrange
            int id = 5;

            // Act
            var result = ValidationHelper.IsValidId(id, out ActionResult badRequestResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(badRequestResult, Is.Null);
            });
        }

        [Test]
        public void IsValidId_ReturnsFalse_WhenIdIsZeroOrLess()
        {
            // Arrange
            int id = 0;

            // Act
            var result = ValidationHelper.IsValidId(id, out ActionResult badRequestResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(badRequestResult, Is.Not.Null);
                Assert.That(badRequestResult, Is.InstanceOf<BadRequestObjectResult>());

                var badRequestObjectResult = badRequestResult as BadRequestObjectResult;
                Assert.That(badRequestObjectResult, Is.Not.Null);
                Assert.That(badRequestObjectResult!.Value, Is.InstanceOf<ErrorResponse>());

                var errorResponse = badRequestObjectResult.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse!.Message, Is.EqualTo("Invalid Id"));
                Assert.That(errorResponse.Detail, Is.EqualTo("The provided Id must be greater than zero."));
            });
        }

        [Test]
        public void IsValidApplication_ReturnsFalse_WhenApplicationIsInvalid()
        {
            foreach (var testcase in InvalidApplications())
            {
                // Arrange
                var app = testcase.Arguments[0] as ApplicationDto;
                var result = testcase.Arguments[1] as string;
                
                // Act
                var isValid = ValidationHelper.IsValidApplication(app!, out ActionResult badRequestResult);

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(isValid, Is.False);
                    Assert.That(badRequestResult, Is.Not.Null);
                    Assert.That(badRequestResult, Is.InstanceOf<BadRequestObjectResult>());

                    var badRequestObjectResult = badRequestResult as BadRequestObjectResult;
                    Assert.That(badRequestObjectResult, Is.Not.Null);
                    Assert.That(badRequestObjectResult!.Value, Is.InstanceOf<ErrorResponse>());

                    var errorResponse = badRequestObjectResult.Value as ErrorResponse;
                    Assert.That(errorResponse, Is.Not.Null);
                    Assert.That(errorResponse!.Message, Is.EqualTo("Invalid Application"));
                    Assert.That(errorResponse.Detail.Trim(), Is.EqualTo(result));
                });
            }
        }



        [Test]
        public void IsValidApplication_ReturnsTrue_WhenApplicationIsValid()
        {
            // Arrange
            var application = new ApplicationDto
            {
                Source = new SourceDto { Name = "Source" },
                Organization = new OrganizationDto { Name = "Organization" },
                JobTitle = new JobTitleDto { Name = "JobTitle" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "WorkEnvironment" }
            };

            // Act 
            var isValid = ValidationHelper.IsValidApplication(application, out ActionResult badRequestResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(isValid, Is.True);
                Assert.That(badRequestResult, Is.Null);
            });

        }

        [Test]
        public void IsValidAppEvent_ReturnsFalse_WhenAppEventIsInvalid()
        {
            foreach(var testcase in InvalidAppEvents())
            {
                // Arrange
                var appEvent = testcase.Arguments[0] as AppEventDto;
                var result = testcase.Arguments[1] as string;
                // Act
                var isValid = ValidationHelper.IsValidAppEvent(appEvent!, out ActionResult badRequestResult);
                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(isValid, Is.False);
                    Assert.That(badRequestResult, Is.Not.Null);
                    Assert.That(badRequestResult, Is.InstanceOf<BadRequestObjectResult>());
                    
                    var badRequestObjectResult = badRequestResult as BadRequestObjectResult;
                    Assert.That(badRequestObjectResult, Is.Not.Null);
                    Assert.That(badRequestObjectResult!.Value, Is.InstanceOf<ErrorResponse>());
                    
                    var errorResponse = badRequestObjectResult.Value as ErrorResponse;
                    Assert.That(errorResponse, Is.Not.Null);
                    Assert.That(errorResponse!.Message, Is.EqualTo("Invalid AppEvent"));
                    Assert.That(errorResponse.Detail.Trim(), Is.EqualTo(result));
                });
            }
        }


        [Test]
        public void IsValidAppEvent_ReturnsTrue_WhenAppEventIsValid()
        {
            // Arrange
            var appEvent = new AppEventDto
            {
                ApplicationId = 1,
                EventDate = new DateTime(2022, 1, 1),
                EventType = EventType.Interview.ToString(),
                ContactMethod = ContactMethod.Call.ToString(),
                Description = new string('X', 999),
            };
            
            // Act
            var isValid = ValidationHelper.IsValidAppEvent(appEvent, out ActionResult badRequestResult);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(isValid, Is.True);
                Assert.That(badRequestResult, Is.Null);
            });
        }

        private static IEnumerable<TestCaseData> InvalidApplications()
        {
            yield return new TestCaseData(
                new ApplicationDto
                {
                    Organization = new OrganizationDto { Name = "Organization" },
                    JobTitle = new JobTitleDto { Name = "JobTitle" },
                    WorkEnvironment = new WorkEnvironmentDto { Name = "WorkEnvironment" }
                },
                "Source is a required value for a Application")
            .SetName("invalid_source");

            yield return new TestCaseData(
                new ApplicationDto
                {
                    Source = new SourceDto { Name = "Source" },
                    JobTitle = new JobTitleDto { Name = "JobTitle" },
                    WorkEnvironment = new WorkEnvironmentDto { Name = "WorkEnvironment" }
                },
                "Organization is a required value for a Application")
            .SetName("invalid_organization");

            yield return new TestCaseData(
                new ApplicationDto
                {
                    Source = new SourceDto { Name = "Source" },
                    Organization = new OrganizationDto { Name = "Organization" },
                    WorkEnvironment = new WorkEnvironmentDto { Name = "WorkEnvironment" }
                },
                "JobTitle is a required value for a Application"
            ).SetName("invalid_jobtitle");

            yield return new TestCaseData(
                new ApplicationDto
                {
                    Source = new SourceDto { Name = "Source" },
                    Organization = new OrganizationDto { Name = "Organization" },
                    JobTitle = new JobTitleDto { Name = "JobTitle" },
                },
                "WorkEnvironment is a required value for a Application"
            ).SetName("invalid_workenvironment");
        }
        private static IEnumerable<TestCaseData> InvalidAppEvents()
        {
            yield return new TestCaseData(
                new AppEventDto
                {
                    ApplicationId = 0,
                    EventDate = new DateTime(2022, 1, 1),
                    EventType = EventType.Interview.ToString(),
                    ContactMethod = ContactMethod.Call.ToString(),
                    Description = new string('X', 1000),
                },
                "ApplicationId is a required value for a AppEvent"
            ).SetName("invalid_application");

            yield return new TestCaseData(
                new AppEventDto
                {
                    ApplicationId = 1,
                    EventDate = default,
                    EventType = EventType.Interview.ToString(),
                    ContactMethod = ContactMethod.Call.ToString(),
                    Description = new string('X', 1000),
                },
                "EventDate is a required value for a AppEvent"
            ).SetName("invalid_date");

            yield return new TestCaseData(
                new AppEventDto
                {
                    ApplicationId = 1,
                    EventDate = new DateTime(2022, 1, 1),
                    EventType = "invalid",
                    ContactMethod = ContactMethod.Call.ToString(),
                    Description = new string('X', 1000),
                },
                $"A valid EventType is required. Expected values {string.Join(", ", Enum.GetNames(typeof(EventType)))}"
            ).SetName("invalid_eventtype");

            yield return new TestCaseData(
                new AppEventDto
                {
                    ApplicationId = 1,
                    EventDate = new DateTime(2022, 1, 1),
                    EventType = EventType.Interview.ToString(),
                    ContactMethod = "invalid",
                    Description = new string('X', 1000),
                },
                $"A valid ContactMethod is required. Expected values {string.Join(", ", Enum.GetNames(typeof(ContactMethod)))}"
            ).SetName("invalid_contactmethod");

            yield return new TestCaseData(
                new AppEventDto
                {
                    ApplicationId = 1,
                    EventDate = new DateTime(2022, 1, 1),
                    EventType = EventType.Interview.ToString(),
                    ContactMethod = ContactMethod.Call.ToString(),
                    Description = new string('X', 1001),
                },
                "Description must be less than 1000 characters"
            ).SetName("invalid_description");
        }


    }
}
