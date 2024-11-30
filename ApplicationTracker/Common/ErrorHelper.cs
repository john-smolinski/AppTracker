using Microsoft.AspNetCore.Mvc;

namespace ApplicationTracker.Common
{
    public static class ErrorHelper
    {
        public static ObjectResult InternalServerError(string message, string detail)
        {
            return GetObjectResult(message, detail, StatusCodes.Status500InternalServerError);
        }

        public static ObjectResult NotFound(string message, string detail)
        {
            return GetObjectResult(message, detail, StatusCodes.Status404NotFound);
        }

        public static ObjectResult BadRequest(string message, string detail)
        {
            return GetObjectResult(message, detail, StatusCodes.Status400BadRequest);
        }

        private static ObjectResult GetObjectResult(string message, string detail, int statusCode)
        {
            return new ObjectResult(new ErrorResponse
            {
                Message = message,
                StatusCode = statusCode,
                Detail = detail
            })
            {
                StatusCode = statusCode
            };
        }
    }
}
