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

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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
                if(await _applicationService.ExistsAsync(application))
                {
                    _logger.LogWarning("Application already exist");
                    return new ConflictResult();
                }

                var result = await _applicationService.PostAsync(application);
                return CreatedAtAction(nameof(GetApplication), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                var message = $"An unexpected errror occured while posting new Application";
                _logger.LogError(ex, message);
                return ErrorHelper.InternalServerError(message, ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationDto>> UpdateApplication(int id, ApplicationDto application)
        {
            try
            {
                if (application == null)
                {
                    _logger.LogInformation("NULL Application DTO received");
                    return ErrorHelper.BadRequest("Invalid application", "The application DTO cannot be null");
                }

                if (id != application.Id)
                {
                    _logger.LogInformation("Application Id mismatch");
                    return ErrorHelper.BadRequest("Invalid request", "The Id in the URL does not match the Id in the body");
                }

                if (!ValidationHelper.IsValidApplication(application, out var badRequestResult))
                {
                    _logger.LogInformation("Invalid Application provided");
                    return badRequestResult;
                }

                if (!await _applicationService.ExistsAsync(id))
                {
                    _logger.LogWarning("Application not found");
                    return NotFound($"Application with Id {id} not found.");
                }

                var updatedApplication = await _applicationService.UpdateAsync(application);
                return Ok(updatedApplication);
            }
            catch (Exception ex)
            {
                var message = "An unexpected error occurred while updating the application";
                _logger.LogError(ex, message);
                return ErrorHelper.InternalServerError(message, ex.Message);
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}/events")]
        public async Task<ActionResult<IEnumerable<AppEventDto>>> GetApplicationEvents(int id)
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
                var result = await _applicationService.GetEventsAsync(id);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                var message = $"An unexpected error occurred while getting AppEvents for Application with Id {id}";
                _logger.LogError(ex, message);
                return ErrorHelper.InternalServerError(message, ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{applicationId}/events/{eventId}")]
        public async Task<ActionResult<AppEventDto>> GetAppEventById(int applicationId, int eventId)
        {
            try
            {
                var validationError = ValidateRequest(applicationId, eventId);
                if (validationError != null) return validationError;

                if (!await _applicationService.ExistsAsync(applicationId))
                {
                    _logger.LogInformation("Application with id {applicationId} not found", applicationId);
                    return ErrorHelper.NotFound("Application not found", $"No Application with Id {applicationId} found");
                }
                if (!await _applicationService.EventExistsAsync(applicationId, eventId))
                {
                    _logger.LogInformation("Event with id {eventId} not found for Application with id {applicationId}", eventId, applicationId);
                    return ErrorHelper.NotFound("Event not found", $"No Event with Id {eventId} found for Application with Id {applicationId}");
                }
                var result = await _applicationService.GetEventByIdAsync(applicationId, eventId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var message = $"An unexpected error occurred while getting AppEvent with Id {eventId} for Application with Id {applicationId}";
                _logger.LogError(ex, message);
                return ErrorHelper.InternalServerError(message, ex.Message);
            }
        }
        
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("{applicationId}/events")]
        public async Task<ActionResult<AppEventDto>> PostAppEvent(int applicationId, AppEventDto appEvent)
        {
            try
            {
                var validationError = ValidateAppEvent(applicationId, appEvent);
                if (validationError != null) return validationError;

                if (!await _applicationService.ExistsAsync(applicationId))
                {
                    _logger.LogInformation("Application with id {applicationId} not found", applicationId);
                    return ErrorHelper.NotFound("Application not found", $"No Application with Id {applicationId} found");
                }

                var result = await _applicationService.PostEventAsync(applicationId, appEvent);
                return CreatedAtAction(nameof(GetAppEventById), new { applicationId = result.ApplicationId, eventId = result.Id }, result);
            }
            catch (Exception ex)
            {
                var message = $"An unexpected error occurred while posting new AppEvent for Application with Id {applicationId}";
                _logger.LogError(ex, message);
                return ErrorHelper.InternalServerError(message, ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{applicationId}/events/{eventId}")]
        public async Task<ActionResult<AppEventDto>> UpdateAppEvent(int applicationId, int eventId, AppEventDto appEvent)
        {
            try
            {
                var validationError = ValidateAppEvent(applicationId, appEvent);
                if (validationError != null) return validationError;
                
                if(!appEvent.Id.HasValue)
                {
                    _logger.LogInformation("Missing Event Id provided in AppEvent DTO");
                    return ErrorHelper.BadRequest("Invalid AppEvent", "The AppEvent DTO must have an Id value");
                }

                if (appEvent.Id.Value != eventId)
                {
                    _logger.LogInformation("Event Id mismatch");
                    return ErrorHelper.BadRequest("Invalid request", "The Id in the URL does not match the Id in the body");
                }

                if (!await _applicationService.EventExistsAsync(applicationId, appEvent.Id.Value))
                {
                    _logger.LogInformation("AppEvent with id {eventId} not found for Application with id {applicationId}", eventId, applicationId);
                    return ErrorHelper.NotFound("AppEvent not found", $"No Event with Id {eventId} found for Application with Id {applicationId}");
                }

                var result = await _applicationService.UpdateEventAsync(applicationId, appEvent);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var message = $"An unexpected error occurred while updating AppEvent with Id {eventId} for Application with Id {applicationId}";
                _logger.LogError(ex, message);
                return ErrorHelper.InternalServerError(message, ex.Message);
            }
        }


        // delete


        private ActionResult? ValidateRequest(int applicationId, int eventId)
        {
            if (!ValidationHelper.IsValidId(applicationId, out var badRequestResult))
            {
                _logger.LogInformation("Invalid Application Id provided: {applicationId}", applicationId);
                return badRequestResult;
            }
            if (!ValidationHelper.IsValidId(eventId, out badRequestResult))
            {
                _logger.LogInformation("Invalid Event Id provided: {eventId}", eventId);
                return badRequestResult;
            }
            return null;
        }


        /// <summary>
        /// Validate the AppEvent DTO is not null
        /// Validate that the Application Id is a valid value (ValidationHelper.IsValidId)
        /// Validate the AppEvent DTO (ValidationHelper.IsValidAppEvent)
        /// Validate that the Application Id in the URL matches the Application Id in the AppEvent DTO
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="appEvent"></param>
        /// <returns></returns>
        private ActionResult? ValidateAppEvent(int applicationId, AppEventDto appEvent)
        {
            if (appEvent == null)
            {
                _logger.LogInformation("NULL AppEvent DTO received");
                return ErrorHelper.BadRequest("Invalid AppEvent", "The AppEvent DTO cannot be null");
            }
            if (!ValidationHelper.IsValidId(applicationId, out var badRequestResult) ||
                !ValidationHelper.IsValidAppEvent(appEvent, out badRequestResult))
            {
                _logger.LogInformation("Invalid AppEvent posted");
                return badRequestResult;
            }
            if (appEvent.ApplicationId != applicationId)
            {
                _logger.LogInformation("Mismatch between applicationId in URL ({applicationId}) and AppEvent.ApplicationId ({appEvent.ApplicationId})", applicationId, appEvent.ApplicationId);
                return ErrorHelper.BadRequest("Invalid AppEvent", "Mismatch between applicationId in the URL and in the payload.");
            }
            return null;
        }

    }
}
