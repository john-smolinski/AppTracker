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
    public class OrganizationsController : ControllerBase
    {
        private readonly TrackerDbContext _context;
        private readonly ILogger<OrganizationsController> _logger;
        public OrganizationsController(TrackerDbContext context, ILogger<OrganizationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetOrganizations()
        {
            var result = await _context.Organizations
                .Select(x => new OrganizationDto { Id = x.Id, Name = x.Name })
                .ToListAsync();

            if (!result.Any())
            {
                _logger.LogWarning(message: "No Organizations returned");
                return NotFound(new ErrorResponse
                {
                    Message = "Organizations not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = "No Organiazations have been added yet"
                });
            }
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Organization>> GetOrganization(int id)
        {
            var exists = await _context.Organizations.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                _logger.LogInformation(message: $"Organization with id {id} not found");
                return NotFound(new ErrorResponse
                {
                    Message = "Organization not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = $"No Organization with id {id} found"
                });
            }

            var result = await _context.Organizations
                .Where(x => x.Id == id)
                .Select(x => new OrganizationDto { Id = x.Id, Name = x.Name })
                .FirstOrDefaultAsync();

            return Ok(result);
        }
    }
}
