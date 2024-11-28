using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTracker.Services
{
    public class LocationService(TrackerDbContext context, ILogger<LocationService> logger) : IService<LocationDto>
    {
        private readonly TrackerDbContext _context = context;
        private readonly ILogger<LocationService> _logger = logger;

        public async Task<IEnumerable<LocationDto>> GetAllAsync()
        {
            return await _context.Locations
                .Select(x => new LocationDto { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        public async Task<LocationDto?> GetByIdAsync(int id)
        {
            var result = await _context.Locations
                .Where(x => x.Id == id)
                .Select(x => new LocationDto 
                    { Id = x.Id, Name = x.Name, City = x.City, State = x.State, Country = x.Country })
                .FirstOrDefaultAsync();

            if(result == null)
            {
                _logger.LogInformation("No Location with id {id} found", id);
            }
            return result;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Locations.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ApplicationDto>?> GetRelatedApplicationsAsync(int id)
        {
            return await _context.Applications
                .Where(x => x.LocationId == id)
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
    }
}
