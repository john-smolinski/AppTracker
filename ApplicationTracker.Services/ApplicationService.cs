using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Services
{
    public class ApplicationService(TrackerDbContext context, ILogger<ApplicationService> logger) : IApplicationService<ApplicationDto>
    {
        private readonly TrackerDbContext _context = context;
        private readonly ILogger<ApplicationService> _logger = logger;



        public async Task<IEnumerable<ApplicationDto>> GetAllAsync()
        {
            return await _context.Applications
                .Select(x => new ApplicationDto
                {
                    ApplicaitionDate = x.ApplicationDate,
                    Source = new SourceDto { Id = x.SourceId, Name = x.Source.Name },
                    Organization = new OrganizationDto { Id = x.OrganizationId, Name = x.Organization.Name },
                    JobTitle = new JobTitleDto { Id = x.JobTitleId, Name = x.JobTitle.Name },
                    WorkEnvironment = new WorkEnvironmentDto { Id = x.WorkEnvironmentId, Name = x.WorkEnvironment.Name },
                    Location = new LocationDto { Id = x.LocationId, Name = x.Location.Name }
                })
                .ToListAsync();
        }

        public async Task<ApplicationDto?> GetByIdAsync(int id)
        {
            return await _context.Applications
                .Where(x => x.Id == id)
                .Select(x => new ApplicationDto
                {
                    ApplicaitionDate = x.ApplicationDate,
                    Source = new SourceDto { Id = x.SourceId, Name = x.Source.Name },
                    Organization = new OrganizationDto { Id = x.OrganizationId, Name = x.Organization.Name },
                    JobTitle = new JobTitleDto { Id = x.JobTitleId, Name = x.JobTitle.Name },
                    WorkEnvironment = new WorkEnvironmentDto { Id = x.WorkEnvironmentId, Name = x.WorkEnvironment.Name },
                    Location = new LocationDto { Id = x.LocationId, Name = x.Location.Name }
                })
                .FirstOrDefaultAsync();
        }

        public Task<ApplicationDto> PostAsync(ApplicationDto application)
        {
            // TODO 
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Applications.AnyAsync(x => x.Id == id);
        }

    }
}
