using ApplicationTracker.Common;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ApplicationTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkEnvironmentsController(ServiceFactory serviceFactory, ILogger<WorkEnvironmentsController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<WorkEnvironmentsController> _logger = logger;

        /// <summary>
        /// Get a list of all Sources
        /// </summary>
        /// <returns>List of Sources or an error response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkEnvironmentDto>>> GetEnvironments()
        {

            return await ServiceCallHandler.HandleServiceCall<WorkEnvironmentDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    var result = await service.GetAllAsync();
                    if (!result.Any())
                    {
                        _logger.LogWarning("No WorkEnvironments returned. WorkEnvironments are required for the application");
                        return NotFound(new ErrorResponse
                        {
                            Message = "WorkEnvironments missing",
                            StatusCode = StatusCodes.Status404NotFound,
                            Detail = "No WorkEnvironments returned. WorkEnviroments missing or misconfigured"
                        });
                    }
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves a WorkEnvironment by Id
        /// </summary>
        /// <param name="id">The WorkEnvironment Id</param>
        /// <returns>A WorkEnvironment or an error response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkEnvironmentDto>> GetWorkEnvironment(int id)
        {
            if (!ValidationHelper.IsValidId(id, out var badRequestResult))
            {
                _logger.LogInformation("Invalid Id provided; {id}", id);
                return badRequestResult;
            }
            return await ServiceCallHandler.HandleServiceCall<WorkEnvironmentDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    if (!await service.ExistsAsync(id))
                    {
                        _logger.LogInformation("WorkEnvironment with Id {id} not found", id);
                        return ErrorHelper.NotFound("WorkEnvironment not found", $"No WorkEnvironment with Id {id} found");
                    }

                    var result = await service.GetByIdAsync(id);
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves Applications related to a specific WorkEnvironment 
        /// </summary>
        /// <param name="id">The WorkEnvironment Id</param>
        /// <returns>List of applications or an error response.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}/applications")]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetRelatedApplications(int id)
        {
            if (!ValidationHelper.IsValidId(id, out var badRequestResult))
            {
                _logger.LogInformation("Invalid Id provided; {id}", id);
                return badRequestResult;
            }

            return await ServiceCallHandler.HandleServiceCall<WorkEnvironmentDto>(
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
