using ApplicationTracker.Common;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Factory;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobTitlesController(ServiceFactory serviceFactory, ILogger<JobTitlesController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<JobTitlesController> _logger = logger;

        /// <summary>
        /// Get a list of all JobTitles
        /// </summary>
        /// <returns>List of JobTitles or an error response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobTitleDto>>> GetJobTitles()
        {
            return await ServiceCallHandler.HandleServiceCall<JobTitleDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    var result = await service.GetAllAsync();
                    if (!result.Any())
                    {
                        _logger.LogWarning("No JobTitles returned");
                        return NotFound(new ErrorResponse
                        {
                            Message = "JobTitles not found",
                            StatusCode = StatusCodes.Status404NotFound,
                            Detail = "No JobTitles have been added yet"
                        });
                    }
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves a Job Title by Id
        /// </summary>
        /// <param name="id">The JobTitle id</param>
        /// <returns>A Job title or an error response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<JobTitleDto>> GetJobTitle(int id)
        {
            if(!ValidationHelper.IsValidId(id, out var badRequestResult))
            {
                _logger.LogInformation("Invalid Id provided; {id}", id);
                return badRequestResult;
            }

            return await ServiceCallHandler.HandleServiceCall<JobTitleDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    if (!await service.ExistsAsync(id))
                    {
                        _logger.LogInformation("JobTitle with Id {id} not found", id);
                        return ErrorHelper.NotFound("JobTitle not found", $"No JobTitle with Id {id} found");
                    }

                    var result = await service.GetByIdAsync(id);
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves Applications related to a specific JobTitle 
        /// </summary>
        /// <param name="id">The JobTitle Id</param>
        /// <returns>List of applications or an error response.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}/applications")]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetRelatedApplications(int id)
        {
            if(!ValidationHelper.IsValidId(id, out var badRequestResult))
            {
                _logger.LogInformation("Invalid Id provided; {id}", id);
                return badRequestResult;
            }

            return await ServiceCallHandler.HandleServiceCall<JobTitleDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    var result = await service.GetRelatedApplicationsAsync(id);
                    return Ok(result);
                });
        }
    }
}
