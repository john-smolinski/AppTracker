using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationTracker.Common
{
    public static class ErrorHelper
    {
        public static ObjectResult InternalServerError(string message, string detail)
        {
            return new ObjectResult(new ErrorResponse
            {
                Message = message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Detail = detail
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        public static ObjectResult NotFound(string message, string detail)
        {
            return new NotFoundObjectResult(new ErrorResponse
            {
                Message = message,
                StatusCode = StatusCodes.Status404NotFound,
                Detail = detail
            });
        }

        public static ObjectResult BadRequest(string message, string detail)
        {
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = message,
                StatusCode = StatusCodes.Status400BadRequest,
                Detail = detail
            });
        }
    }
}
