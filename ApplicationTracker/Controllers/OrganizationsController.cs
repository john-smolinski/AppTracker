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
    public class OrganizationsController(ServiceFactory serviceFactory, ILogger<OrganizationsController> logger) : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory = serviceFactory;
        private readonly ILogger<OrganizationsController> _logger = logger;

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetOrganizations()
        {
            try
            {
                return await ServiceCallHandler.HandleServiceCall<OrganizationDto>(
                    _serviceFactory,
                    _logger,
                    async service =>
                    {
                        var result = await service.GetAllAsync();
                        if (!result.Any())
                        {
                            _logger.LogWarning("No Organizations returned");
                            return NotFound(new ErrorResponse
                            {
                                Message = "Organizations not found",
                                StatusCode = StatusCodes.Status404NotFound,
                                Detail = "No Organizations have been added yet"
                            });
                        }
                        return Ok(result);
                    });
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, "Service not found");
                return ErrorHelper.InternalServerError("Service not found", ioe.Message);
            }
            catch (ArgumentException ae)
            {
                _logger.LogError(ae, "Service not registered in ServiceFactory");
                return ErrorHelper.InternalServerError("Service not registered in ServiceFactory", ae.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception has occured");
                return ErrorHelper.InternalServerError("Unhandled exception has occured", ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDto>> GetOrganization(int id)
        {
            if (id <= 0)
            {
                _logger.LogInformation("Invalid Id provided; {id}", id);
                return ErrorHelper.BadRequest("Invalid Id", "The provided ID must be greater than zero.");
            }
            try
            {
                var service = _serviceFactory.GetService<OrganizationDto>();

                if (!await service.ExistsAsync(id))
                {
                    _logger.LogInformation(message: "Organization with id {id} not found", id);
                    return ErrorHelper.NotFound("Organization not found", $"No Organization with id {id} found");
                }
                var result = await service.GetByIdAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, "Service not found");
                return ErrorHelper.InternalServerError("Service not found", ioe.Message);
            }
            catch (ArgumentException ae)
            {
                _logger.LogError(ae, "Service not registered in ServiceFactory");
                return ErrorHelper.InternalServerError("Service not registered in ServiceFactory", ae.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception has occured");
                return ErrorHelper.InternalServerError("Unhandled exception has occured", ex.Message);
            }
        }
    }
}
