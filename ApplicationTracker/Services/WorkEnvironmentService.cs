﻿using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services.Interfaces;
using System.Runtime.CompilerServices;
using Serilog;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<ApplicationDto>?> GetReleatedApplicationsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}