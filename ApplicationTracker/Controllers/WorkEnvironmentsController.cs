using ApplicationTracker.Common;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ApplicationTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkEnvironmentsController(ServiceFactory serviceFactory, ILogger<WorkEnvironmentsController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<WorkEnvironmentsController> _logger = logger;

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkEnvironmentDto>>> GetEnvironments()
        {
            var service = _serviceFactory.GetService<WorkEnvironmentDto>();
            var result = await service.GetAllAsync();
            
            if (!result.Any())
            {
                _logger.LogWarning(message: "No WorkEnvironments returned. WorkEnvironments are required for the application");
                return NotFound(new ErrorResponse
                {
                    Message = "WorkEnvironments missing",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = "No WorkEnvironments returned. WorkEnviroments missing or misconfigured"
                });                
            }
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkEnvironmentDto>> GetWorkEnvironment(int id)
        {
            var service = _serviceFactory.GetService<WorkEnvironmentDto>();
            
            if(!await service.ExistsAsync(id))
            {
                _logger.LogInformation(message: "No WorkEnvironment with id {id} found", id);
                return NotFound(new ErrorResponse
                {
                    Message = "WorkEnvironment not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = $"No WorkEnvironment with id {id} found"
                });
            }
            var result = await service.GetByIdAsync(id);

            return Ok(result);
        }
    }
}
