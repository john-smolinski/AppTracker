using ApplicationTracker.Common;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ApplicationTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkEnvironmentsController : ControllerBase
    {
        private readonly TrackerDbContext _context;
        private readonly ILogger<WorkEnvironmentsController> _logger;

        public WorkEnvironmentsController(TrackerDbContext context, ILogger<WorkEnvironmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkEnvironmentDto>>> GetEnvironments()
        {
            var result = await _context.WorkEnvironments
                .Select(x => new WorkEnvironmentDto { Id = x.Id, Name = x.Name })
                .OrderBy(x => x.Id)
                .ToListAsync();
            
            if (result != null && result.Any())
            {
                return Ok(result);
            }
            else
            {
                _logger.LogError(message: "No WorkEnvironments returned. WorkEnvironments are required for the application to function");
                return NotFound(new ErrorResponse
                {
                    Message = "WorkEnvironments empty or missing",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = "No WorkEnvironments returned. WorkEnviroments missing or missconfigured"
                });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkEnvironmentDto>> GetWorkEnvironment(int id)
        {
            var workEnvironment = await _context.WorkEnvironments
                .Where(x => x.Id == id)
                .Select(x => new WorkEnvironmentDto { Id = x.Id, Name = x.Name })
                .FirstOrDefaultAsync();

            if (workEnvironment == null)
            {
                _logger.LogWarning(message: $"No WorkEnvironment with id {id} found");
                return NotFound(new ErrorResponse
                {
                    Message = $"WorkEnvironment not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = $"No WorkEnvironment with id {id} not found"
                });
            }
            return Ok(workEnvironment);
        }
    }
}
