using ApplicationTracker.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ApplicationTracker.Common
{
    public static class ValidationHelper
    {
        public static bool IsValidId(int id, out ActionResult badRequestResult)
        {
            if (id > 0)
            {
                badRequestResult = null!;
                return true;
            }
            badRequestResult = ErrorHelper.BadRequest("Invalid ID", "The provided ID must be greater than zero.");
            return false;
        }

        public static bool IsValidApplication(ApplicationDto application, out ActionResult badRequestResult)
        {
            var badRequest = new StringBuilder();
            
            if(string.IsNullOrEmpty(application.Source.Name))
            {
                badRequest.AppendLine("Source is a required value for a Application");
            }
            if(string.IsNullOrEmpty(application.Organization.Name))
            {
                badRequest.AppendLine("Organization is a required value for a Application");
            }
            if(string.IsNullOrEmpty(application.JobTitle.Name))
            {
                badRequest.AppendLine("JobTitle is a required value for a Application");
            }
            if(string.IsNullOrEmpty(application.WorkEnvironment.Name))
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
    }
}
