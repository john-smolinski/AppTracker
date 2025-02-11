using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationTracker.Services
{
    public class JobTitleService(TrackerDbContext context, ILogger<JobTitleService> logger) : IService<JobTitleDto>
    {
        private readonly TrackerDbContext _context = context;
        private readonly ILogger<JobTitleService> _logger = logger;

        public async Task<IEnumerable<JobTitleDto>> GetAllAsync()
        {
            return await _context.JobTitles
                .AsNoTracking()
                .Select(x => new JobTitleDto { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        public async Task<JobTitleDto?> GetByIdAsync(int id)
        {
            var result = await _context.JobTitles
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new JobTitleDto { Id = x.Id, Name = x.Name })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                _logger.LogInformation("No JobTitle with id {id} found", id);
            }
            return result;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.JobTitles.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ApplicationDto>?> GetRelatedApplicationsAsync(int id)
        {
            return await _context.Applications
                .AsNoTracking()
                .Where(x => x.JobTitleId == id)
                .Select(x => new ApplicationDto 
                { 
                    ApplicationDate = x.ApplicationDate, 
                    Source = new SourceDto 
                    { 
                        Id = x.SourceId, Name = x.Source.Name 
                    },
                    Organization = new OrganizationDto 
                    { 
                        Id = x.OrganizationId, Name = x.Organization.Name 
                    },
                    JobTitle = new JobTitleDto 
                    { 
                        Id = x.JobTitleId, Name = x.JobTitle.Name 
                    },
                    WorkEnvironment = new WorkEnvironmentDto 
                    { 
                        Id = x.WorkEnvironmentId, Name = x.WorkEnvironment.Name 
                    },
                    City = x.City,
                    State = x.State

                })
                .ToListAsync();
        }
    }
}
