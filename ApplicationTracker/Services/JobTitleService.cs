using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTracker.Services
{
    public class JobTitleService(TrackerDbContext context, ILogger<JobTitleService> logger) : IService<JobTitleDto>
    {
        private readonly TrackerDbContext _context = context;
        private readonly ILogger<JobTitleService> _logger = logger;

        public async Task<IEnumerable<JobTitleDto>> GetAllAsync()
        {
            return await _context.JobTitles
                .Select(x => new JobTitleDto { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        public async Task<JobTitleDto?> GetByIdAsync(int id)
        {
            var result = await _context.JobTitles
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

        public Task<IEnumerable<ApplicationDto>?> GetReleatedApplicationsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
