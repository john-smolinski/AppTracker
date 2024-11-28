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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations()
        {
            var service = _serviceFactory.GetService<LocationDto>();
            var result = await service.GetAllAsync();

            if (!result.Any())
            {
                _logger.LogWarning(message: "No Locations returned.");
                return NotFound(new ErrorResponse
                {
                    Message = "Locations not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = "No Locations have been added yet."
                });
            }
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetLocation(int id)
        {
            var service = _serviceFactory.GetService<LocationDto>();

            if (!await service.ExistsAsync(id))
            {
                _logger.LogInformation(message: "No Location with id {id} found", id);
                return NotFound(new ErrorResponse
                {
                    Message = "Location not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail =$"No Location with id {id} found"
                });
            }
            var result = await service.GetByIdAsync(id);

            return Ok(result);
        }
    }
}
