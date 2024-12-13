using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApplicationTracker.Common;

namespace ApplicationTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController(IApplicationService<ApplicationDto> applicationService,  ILogger<ApplicationsController> logger) : ControllerBase
    {
        private readonly IApplicationService<ApplicationDto> _applicationService = applicationService;
        private readonly ILogger<ApplicationsController> _logger = logger;

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetAllApplications()
        {
            try
            {
                var result = await _applicationService.GetAllAsync();
                if (!result.Any())
                {
                    _logger.LogInformation("No Applications found");
                    return NotFound(new ErrorResponse
                    {
                        Message = "Applications not found",
                        StatusCode = StatusCodes.Status404NotFound,
                        Detail = "No Applications have been added yet"
                    });
                }
                return Ok(result);
            }
            catch (Exception ex) 
            {
                var message = "An unexpected error occured while fetching all applications";
                _logger.LogError(ex, message);
                return ErrorHelper.InternalServerError(message, ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationDto>> GetApplication(int id)
        {
            try
            {
                if (!ValidationHelper.IsValidId(id, out var badRequestResult))
                {
                    _logger.LogInformation("Invalid Id provided: {id}", id);
                    return badRequestResult;
                }

                if (!await _applicationService.ExistsAsync(id))
                {
                    _logger.LogInformation("Application with id {id} not found", id);
                    return ErrorHelper.NotFound("Application not found", $"No Application with Id {id} found");
                }

                var result = await _applicationService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var message = $"An unexpected error occured while fetching Application with Id {id}";
                _logger.LogError(ex, message);
                return ErrorHelper.InternalServerError(message, ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<ApplicationDto>> PostNewApplication(ApplicationDto application)
        {
            try
            {
                if(application == null)
                {
                    _logger.LogInformation("NULL Applicaton DTO recieved");
                    return ErrorHelper.BadRequest("Invalid application", "The application DTO cannot be null");
                }
                if (!ValidationHelper.IsValidApplication(application, out var badRequestResult))
                {
                    _logger.LogInformation("Invalid Application posted");
                    return badRequestResult;
                }
                var result = await _applicationService.PostAsync(application);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var message = $"An unexpected errror occured while posting new Application";
                _logger.LogError(ex, message);
                return ErrorHelper.InternalServerError(message, ex.Message);
            }
        }
    }
}
