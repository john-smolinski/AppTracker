using Microsoft.AspNetCore.Mvc;

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
    }
}
