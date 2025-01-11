using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationTracker.Services
{
    public class WorkEnvironmentService(TrackerDbContext context, ILogger<WorkEnvironmentService> logger) : IService<WorkEnvironmentDto>
    {
        private readonly TrackerDbContext _context = context;
        private readonly ILogger<WorkEnvironmentService> _logger = logger;

        public async Task<IEnumerable<WorkEnvironmentDto>> GetAllAsync()
        {
            return await _context.WorkEnvironments
                .Select(x => new WorkEnvironmentDto { Id = x.Id , Name = x.Name })
                .ToListAsync();
        }

        public async Task<WorkEnvironmentDto?> GetByIdAsync(int id)
        {
            var result = await _context.WorkEnvironments
                .Where(x => x.Id == id)
                .Select(x => new WorkEnvironmentDto { Id = x.Id, Name = x.Name })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                _logger.LogInformation("No WorkEnvironment with id {id} found", id);
            }
            return result;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.WorkEnvironments.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ApplicationDto>?> GetRelatedApplicationsAsync(int id)
        {
            return await _context.Applications
                .Where(x => x.WorkEnvironmentId == id)
                .Select(x => new ApplicationDto
                {
                    ApplicaitionDate = x.ApplicationDate,
                    Source = new SourceDto 
                    { 
                        Id = x.SourceId, 
                        Name = x.Source.Name 
                    },
                    Organization = new OrganizationDto 
                    { 
                        Id = x.OrganizationId, 
                        Name = x.Organization.Name 
                    },
                    JobTitle = new JobTitleDto 
                    { 
                        Id = x.JobTitleId, 
                        Name = x.JobTitle.Name 
                    },
                    WorkEnvironment = new WorkEnvironmentDto 
                    { 
                        Id = x.WorkEnvironmentId, 
                        Name = x.WorkEnvironment.Name 
                    },
                    Location = x.Location == null 
                        ? null 
                        : new LocationDto 
                        { 
                            Id = x.LocationId, Name = x.Location.Name 
                        }
                })
                .ToListAsync();
        }
    }
}
