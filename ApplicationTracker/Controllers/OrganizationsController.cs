using ApplicationTracker.Common;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ApplicationTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController(ServiceFactory serviceFactory, ILogger<OrganizationsController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<OrganizationsController> _logger = logger;

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetOrganizations()
        {
            var service = _serviceFactory.GetService<OrganizationDto>();
            var result = await service.GetAllAsync();

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
        public async Task<ActionResult<OrganizationDto>> GetOrganization(int id)
        {
            var service = _serviceFactory.GetService<OrganizationDto>();
            if (!await service.ExistsAsync(id))
            {
                _logger.LogInformation(message: "Organization with id {id} not found", id);
                return NotFound(new ErrorResponse
                {
                    Message = "Organization not found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Detail = $"No Organization with id {id} found"
                });
            }
            var result = await service.GetByIdAsync(id);

            return Ok(result);
        }
    }
}
