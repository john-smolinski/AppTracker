using ApplicationTracker.Common;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Factory;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourcesController(ServiceFactory serviceFactory, ILogger<SourcesController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<SourcesController> _logger = logger;

        /// <summary>
        /// Get a list of all Sources
        /// </summary>
        /// <returns>List of Sources or an error response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SourceDto>>> GetSources()
        {
            return await ServiceCallHandler.HandleServiceCall<SourceDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    var result = await service.GetAllAsync();
                    if (!result.Any())
                    {
                        _logger.LogWarning("No Sources returned");
                        return NotFound(new ErrorResponse
                        {
                            Message = "Sources missing",
                            StatusCode = StatusCodes.Status404NotFound,
                            Detail = "No Sources returned. Sources missing or misconfigured"
                        });
                    }
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves a Source by Id
        /// </summary>
        /// <param name="id">The Source Id</param>
        /// <returns>A Source or an error response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<SourceDto>> GetSource(int id)
        {
            if (!ValidationHelper.IsValidId(id, out var badRequestResult))
            {
                _logger.LogInformation("Invalid Id provided; {id}", id);
                return badRequestResult;
            }
            return await ServiceCallHandler.HandleServiceCall<SourceDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    if (!await service.ExistsAsync(id))
                    {
                        _logger.LogInformation("Source with Id {id} not found", id);
                        return ErrorHelper.NotFound("Source not found", $"No Source with Id {id} found");
                    }

                    var result = await service.GetByIdAsync(id);
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves Applications related to a specific Source 
        /// </summary>
        /// <param name="id">The Source Id</param>
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

            return await ServiceCallHandler.HandleServiceCall<SourceDto>(
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
