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
    public class LocationsController(ServiceFactory serviceFactory, ILogger<LocationsController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<LocationsController> _logger = logger;

        /// <summary>
        /// Get a list of all Locations
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations()
        {

            return await ServiceCallHandler.HandleServiceCall<LocationDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    var result = await service.GetAllAsync();
                    if (!result.Any())
                    {
                        _logger.LogWarning("No Locations returned");
                        return NotFound(new ErrorResponse
                        {
                            Message = "Locations not found",
                            StatusCode = StatusCodes.Status404NotFound,
                            Detail = "No Locations have been added yett"
                        });
                    }
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves a Location by Id
        /// </summary>
        /// <param name="id">The Location Id</param>
        /// <returns>A Location or an error response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetLocation(int id)
        {
            if (!ValidationHelper.IsValidId(id, out var badRequestResult))
            {
                _logger.LogInformation("Invalid Id provided; {id}", id);
                return badRequestResult;
            }

            return await ServiceCallHandler.HandleServiceCall<LocationDto>(
                _serviceFactory,
                _logger,
                async service =>
                {
                    if (!await service.ExistsAsync(id))
                    {
                        _logger.LogInformation("Location with Id {id} not found", id);
                        return ErrorHelper.NotFound("Location not found", $"No Location with Id {id} found");
                    }

                    var result = await service.GetByIdAsync(id);
                    return Ok(result);
                });
        }

        /// <summary>
        /// Retrieves Applications related to a specific Location 
        /// </summary>
        /// <param name="id">The Location Id</param>
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

            return await ServiceCallHandler.HandleServiceCall<LocationDto>(
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
