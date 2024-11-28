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
    public class JobTitlesController(ServiceFactory serviceFactory, ILogger<JobTitlesController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<JobTitlesController> _logger = logger;

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobTitleDto>>> GetJobTitles()
        {
            var service = _serviceFactory.GetService<JobTitleDto>();
            var result = await service.GetAllAsync();

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
            var service = _serviceFactory.GetService<JobTitleDto>();
            
            if (!await service.ExistsAsync(id))
            {
                _logger.LogInformation(message: "JobTitle with id {id} not found", id);
                return NotFound(new ErrorResponse
                {
                    Message = "JobTitle not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = $"No JobTitle with id {id} found"
                });
            }
            var result = await service.GetByIdAsync(id);

            return Ok(result);
        }
    }
}
