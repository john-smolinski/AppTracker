﻿using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationTracker.Services
{
    public class OrganizationService(TrackerDbContext context, ILogger<OrganizationService> logger) : IService<OrganizationDto>
    {
        private readonly TrackerDbContext _context = context;
        private readonly ILogger<OrganizationService> _logger = logger;

        public async Task<IEnumerable<OrganizationDto>> GetAllAsync()
        {
            return await _context.Organizations
                .AsNoTracking()
                .Select(x => new OrganizationDto { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        public async Task<OrganizationDto?> GetByIdAsync(int id)
        {
            var result = await _context.Organizations
                .AsNoTracking()
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

        public async Task<IEnumerable<ApplicationDto>?> GetRelatedApplicationsAsync(int id)
        {
            return await _context.Applications
                .AsNoTracking()
                .Where(x => x.OrganizationId == id)
                .Select(x => new ApplicationDto
                {
                    ApplicationDate = x.ApplicationDate,
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
