using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<ApplicationDto>?> GetReleatedApplicationsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
