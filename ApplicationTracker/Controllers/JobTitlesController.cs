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
    public class JobTitlesController : ControllerBase
    {
        private readonly TrackerDbContext _context;
        private readonly ILogger<JobTitlesController> _logger;

        public JobTitlesController(TrackerDbContext context, ILogger<JobTitlesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobTitleDto>>> GetJobTitles()
        {
            var result = await _context.JobTitles
                .Select(x => new JobTitleDto { Id = x.Id, Name = x.Name })
                .ToListAsync();

            if (!result.Any())
            {
                _logger.LogWarning(message: "No JobTitles returned");
                return NotFound(new ErrorResponse
                {
                    Message = "JobTitles not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = "No JobTitles have been added yet"
                });
            }
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<JobTitleDto>> GetJobTitle(int id)
        {
            var exists = await _context.JobTitles.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                _logger.LogInformation(message: $"JobTitle with id {id} not found");
                return NotFound(new ErrorResponse
                {
                    Message = "JobTitle not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = $"No JobTitle with id {id} found"
                });
            }

            var result = await _context.JobTitles
                .Where(x => x.Id == id)
                .Select(x => new JobTitleDto { Id = x.Id, Name = x.Name })
                .FirstOrDefaultAsync();

            return Ok(result);
        }
    }
}
