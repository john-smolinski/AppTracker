using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTracker.Services
{
    public class OrganizationService(TrackerDbContext context, ILogger<OrganizationService> logger) : IService<OrganizationDto>
    {
        private readonly TrackerDbContext _context = context;
        private readonly ILogger<OrganizationService> _logger = logger;

        public async Task<IEnumerable<OrganizationDto>> GetAllAsync()
        {
            return await _context.Organizations
                .Select(x => new OrganizationDto { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        public async Task<OrganizationDto?> GetByIdAsync(int id)
        {
            var result = await _context.Organizations
                .Where(x => x.Id == id)
                .Select(x => new OrganizationDto { Id = x.Id, Name = x.Name})
                .FirstOrDefaultAsync();

            if(result == null)
            {
                _logger.LogInformation("No Organizations with id {id} found", id);
            }
            return result;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Organizations.AnyAsync(x => x.Id == id);
        }
        public async Task<IEnumerable<ApplicationDto>?> GetReleatedApplicationsAsync(int id)
        {
            throw new NotImplementedException();
        }

    }
}
