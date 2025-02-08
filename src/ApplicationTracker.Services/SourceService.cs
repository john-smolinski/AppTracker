using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationTracker.Services
{
    public class SourceService(TrackerDbContext context, ILogger<SourceService> logger) : IService<SourceDto>
    {
        private readonly TrackerDbContext _context = context;
        private readonly ILogger<SourceService> _logger = logger;

        public async Task<IEnumerable<SourceDto>> GetAllAsync()
        {
            return await _context.Sources
                .Select(x => new SourceDto { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        public async Task<SourceDto?> GetByIdAsync(int id)
        {
            var result = await _context.Sources
                .Where(x => x.Id == id)
                .Select(x => new SourceDto { Id = x.Id, Name= x.Name })
                .FirstOrDefaultAsync();
            
            if(result == null)
            {
                _logger.LogInformation("No Sources with id {id} found", id);
            }
            return result;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Sources.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ApplicationDto>?> GetRelatedApplicationsAsync(int id)
        {
            return await _context.Applications
                .Where(x => x.SourceId == id)
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
                    City = x.City,
                    State = x.State
                })
                .ToListAsync();
        }
    }
}
