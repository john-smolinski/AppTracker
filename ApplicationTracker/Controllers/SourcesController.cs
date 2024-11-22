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
    public class SourcesController : ControllerBase
    {
        private readonly TrackerDbContext _context;
        private readonly ILogger<SourcesController> _logger;

        public SourcesController(TrackerDbContext context, ILogger<SourcesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SourceDto>>> GetSources()
        {
            var result = await _context.Sources
                .Select(x => new SourceDto { Id = x.Id, Name = x.Name })
                .OrderBy(x => x.Id)
                .ToListAsync();

            if (!result.Any())
            {
                _logger.LogError(message: "No Sources returned. Source are required for the application");
                return NotFound(new ErrorResponse
                {
                    Message = "Sources missing",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = "No Sources returned. Sources missing or misconfigured"
                });
            }
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Source>> GetSource(int id)
        {
            var exists = await _context.Sources.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                _logger.LogWarning(message: $"No Source with id {id} found");
                return NotFound(new ErrorResponse
                {
                    Message = $"Source not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = $"No Surce with id {id} not found"
                });
            }

            var result = await _context.Sources
                .Where(x => x.Id == id)
                .Select(x => new SourceDto { Id = x.Id, Name = x.Name })
                .FirstOrDefaultAsync();

            return Ok(result);
        }
    }
}
