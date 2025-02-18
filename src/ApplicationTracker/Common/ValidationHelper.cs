using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ApplicationTracker.Common
{
    public static class ValidationHelper
    {

        /// <summary>
        /// Validates that the provided Id is greater than zero
        /// </summary>
        /// <param name="id">The Id to validate</param>
        /// <param name="badRequestResult">BadRequestObjectResult or null</param>
        /// <returns>true if valid</returns>
        public static bool IsValidId(int id, out ActionResult badRequestResult)
        {
            if (id > 0)
            {
                badRequestResult = null!;
                return true;
            }
            badRequestResult = ErrorHelper.BadRequest("Invalid Id", "The provided Id must be greater than zero.");
            return false;
        }

        /// <summary>
        /// Validates that the provided Application is valid
        /// </summary>
        /// <param name="application">ApplicationDto to validate</param>
        /// <param name="badRequestResult">BadRequestObjectResult or null</param>
        /// <returns>true if valid</returns>
        public static bool IsValidApplication(ApplicationDto application, out ActionResult badRequestResult)
        {
            var badRequest = new StringBuilder();
            
            if(application.Source == null || string.IsNullOrEmpty(application.Source.Name))
            {
                badRequest.AppendLine("Source is a required value for a Application");
            }
            if(application.Organization == null || string.IsNullOrEmpty(application.Organization.Name))
            {
                badRequest.AppendLine("Organization is a required value for a Application");
            }
            if(application.JobTitle == null || string.IsNullOrEmpty(application.JobTitle.Name))
            {
                badRequest.AppendLine("JobTitle is a required value for a Application");
            }
            if(application.WorkEnvironment == null || string.IsNullOrEmpty(application.WorkEnvironment.Name))
            {
                badRequest.AppendLine("WorkEnvironment is a required value for a Application");
            }
            if (badRequest.Length == 0)
            {
                badRequestResult = null!;
                return true;
            }
            badRequestResult = ErrorHelper.BadRequest("Invalid Application", badRequest.ToString());
            return false;
        }

        /// <summary>
        /// Validates that the provided AppEvent is valid
        /// </summary>
        /// <param name="appEvent">AppEventDto to validate</param>
        /// <param name="badRequestResult">BadRequestObjectResult or null</param>
        /// <returns>true if valid</returns>
        public static bool IsValidAppEvent(AppEventDto appEvent, out ActionResult badRequestResult)
        {
            var badRequest = new StringBuilder();
            
            if (appEvent.ApplicationId <= 0)
            {
                badRequest.AppendLine("ApplicationId is a required value for a AppEvent");
            }
            if(appEvent.EventDate == default)
            {
                badRequest.AppendLine("EventDate is a required value for a AppEvent");
            }
            if(appEvent.EventType == null || !Enum.TryParse<EventType>(appEvent.EventType, out _))
            {
                badRequest.AppendLine($"A valid EventType is required. Expected values {string.Join(", ", Enum.GetNames(typeof(EventType)))}");
            }
            if(appEvent.ContactMethod == null || !Enum.TryParse<ContactMethod>(appEvent.ContactMethod, out _))
            {
                badRequest.AppendLine($"A valid ContactMethod is required. Expected values {string.Join(", ", Enum.GetNames(typeof(ContactMethod)))}");
            }
            if(appEvent.Description != null && appEvent.Description.Length > 1000)
            {
                badRequest.AppendLine("Description must be less than 1000 characters");
            }

            if (badRequest.Length == 0)
            {
                badRequestResult = null!;
                return true;
            }

            badRequestResult = ErrorHelper.BadRequest("Invalid AppEvent", badRequest.ToString());
            return false;
        }
    }
}
