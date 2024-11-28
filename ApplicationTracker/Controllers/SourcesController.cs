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
    public class SourcesController(ServiceFactory serviceFactory, ILogger<SourcesController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<SourcesController> _logger = logger;

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SourceDto>>> GetSources()
        {
            var service = _serviceFactory.GetService<SourceDto>();
            var result = await service.GetAllAsync();

            if (!result.Any())
            {
                _logger.LogWarning(message: "No Sources returned. Sources are required for the application");
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
        public async Task<ActionResult<SourceDto>> GetSource(int id)
        {
            var service = _serviceFactory.GetService<SourceDto>();

            if (!await service.ExistsAsync(id))
            {
                _logger.LogInformation(message: "No Source with id {id} found", id);
                return NotFound(new ErrorResponse
                {
                    Message = "Source not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = $"No Source with id {id} found"
                });
            }
            var result = await service.GetByIdAsync(id);

            return Ok(result);
        }
    }
}
