using ApplicationTracker.Common;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ApplicationTracker.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController(ServiceFactory serviceFactory, ILogger<OrganizationsController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<OrganizationsController> _logger = logger;

        /// <summary>
        /// Get a list of all Organizations
        /// </summary>
        /// <returns>List of Organizations or an error response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetOrganizations()
        {
            return await ServiceCallHandler.HandleServiceCall<OrganizationDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    var result = await service.GetAllAsync();
                    if (!result.Any())
                    {
                        _logger.LogWarning("No Organizations returned");
                        return NotFound(new ErrorResponse
                        {
                            Message = "Organizations not found",
                            StatusCode = StatusCodes.Status404NotFound,
                            Detail = "No Organizations have been added yet"
                        });
                    }
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves a Organization by Id
        /// </summary>
        /// <param name="id">The Organization Id</param>
        /// <returns>A Organization or an error response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDto>> GetOrganization(int id)
        {
            if (!ValidationHelper.IsValidId(id, out var badRequestResult))
            {
                _logger.LogInformation("Invalid Id provided; {id}", id);
                return badRequestResult;
            }

            return await ServiceCallHandler.HandleServiceCall<OrganizationDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    if (!await service.ExistsAsync(id))
                    {
                        _logger.LogInformation("Organization with Id {id} not found", id);
                        return ErrorHelper.NotFound("Organization not found", $"No Organization with Id {id} found");
                    }

                    var result = await service.GetByIdAsync(id);
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves Applications related to a specific Organization 
        /// </summary>
        /// <param name="id">The Organization Id</param>
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

            return await ServiceCallHandler.HandleServiceCall<OrganizationDto>(
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
