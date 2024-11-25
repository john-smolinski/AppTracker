using ApplicationTracker.Common;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ApplicationTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly TrackerDbContext _context;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(TrackerDbContext context, ILogger<LocationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations()
        {
            var result = await _context.Locations
                .Select(x => new LocationDto 
                    { Id = x.Id, Name = x.Name, City = x.City, State = x.State, Country = x.Country })
                .ToListAsync();

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
            var exists = await _context.Locations.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                _logger.LogInformation(message: $"No Location with id {id} found");
                return NotFound(new ErrorResponse
                {
                    Message = "Location not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail =$"No Location with id {id} found"
                });
            }

            var result = await _context.Locations
                .Where(x => x.Id == id)
                .Select(x => new LocationDto
                    { Id = x.Id, Name = x.Name, City = x.City, State = x.State, Country = x.Country })
                .FirstOrDefaultAsync();

            return Ok(result);
        }
    }
}
