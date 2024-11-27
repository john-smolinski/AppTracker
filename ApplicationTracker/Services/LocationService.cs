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

        public Task<IEnumerable<ApplicationDto>?> GetRelatedApplicationsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
