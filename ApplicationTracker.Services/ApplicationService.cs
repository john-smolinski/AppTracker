using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationTracker.Services
{
    public class ApplicationService(TrackerDbContext context, ILogger<ApplicationService> logger) : IApplicationService<ApplicationDto>
    {
        private readonly TrackerDbContext _context = context;
        private readonly ILogger<ApplicationService> _logger = logger;

        public async Task<IEnumerable<ApplicationDto>> GetAllAsync()
        {
            try
            {
                return await _context.Applications
                    .Select(x => MapToDto(x))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while fetching all applications");
                throw;
            }
        }

        public async Task<ApplicationDto?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Applications
                    .Where(x => x.Id == id)
                    .Select(x => MapToDto(x))
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while fetching application with Id {id}", id);
                throw;
            }
        }

        public async Task<ApplicationDto> PostAsync(ApplicationDto application)
        {
            try
            {
                Location? location = null;
                if (application.Location != null)
                {
                    location = await AddLocation(application.Location);
                }

                var newApplication = new Application
                {
                    // test to see if date was supplied. if not *assume* the date is today
                    ApplicationDate = application.ApplicaitionDate == default(DateOnly)
                        ? DateOnly.FromDateTime(DateTime.Now)
                        : application.ApplicaitionDate,
                    
                    Source = await AddEntity<Source>(application.Source),
                    Organization = await AddEntity<Organization>(application.Organization),
                    JobTitle = await AddEntity<JobTitle>(application.JobTitle),
                    WorkEnvironment = await AddEntity<WorkEnvironment>(application.WorkEnvironment),
                    Location = location!
                };
                _context.Applications.Add(newApplication);
                await _context.SaveChangesAsync();

                return MapToDto(newApplication);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new application.");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _context.Applications.AnyAsync(x => x.Id == id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking existence of application with ID {Id}.", id);
                throw;
            }
        }

        private async Task<Location> AddLocation(LocationDto? location)
        {
            try
            {
                // return exisitng location if we have its id
                if (location!.Id.HasValue)
                {
                    var existing = await _context.Locations.FirstOrDefaultAsync(x => x.Id == location.Id.Value);
                    if (existing != null)
                    {
                        return existing;
                    }
                }
                
                // test to see if we have an existing location we can reuse
                var existingLocation = await _context.Locations
                    .FirstOrDefaultAsync(x => object.Equals(x.City, location.City) &&
                                              object.Equals(x.State, location.State));

                if (existingLocation != null) 
                {
                    return existingLocation;
                }

                // location requires at least a state vlue 
                if (location.State == null)
                {
                    return null!;
                }
                
                var newLocation = new Location 
                { 
                    Name = GetLocationName(location.City, location.State!),
                    City = location.City,
                    State = location.State!,
                };

                // save if we're creating a new location
                _context.Locations.Add(newLocation);
                await _context.SaveChangesAsync();
                
                return newLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a location.");
                throw;
            }
        }

        private async Task<T> AddEntity<T>(BaseDto dto)
            where T : BaseEntity, new()
        {
            try
            {
                var dbset = _context.Set<T>();

                if (dto.Id.HasValue)
                {
                    var existing = await dbset.FirstOrDefaultAsync(x => x.Id == dto.Id.Value);
                    if (existing != null)
                    {
                        return existing;
                    }
                }
                else
                {
                    // double check 
                    var existng = await dbset.FirstOrDefaultAsync(x => x.Name == dto.Name);
                    if (existng != null)
                    {
                        return existng;
                    }
                }
                var newEntity = new T
                {
                    Name = dto.Name
                };
                
                _context.Set<T>().Add(newEntity);
                await _context.SaveChangesAsync();
                return newEntity;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding or retrieving an entity of type {EntityType}.", typeof(T).Name);
                throw;
            }
        }

        private static string GetLocationName(string? city, string state)
        {
            var location = string.IsNullOrWhiteSpace(city) ? state : city.Trim();
            return $"{location}|{state.Trim()}";
        }


        private static ApplicationDto MapToDto(Application application)
        {
            return new ApplicationDto
            {
                ApplicaitionDate = application.ApplicationDate,
                Source = new SourceDto { Id = application.SourceId, Name = application.Source?.Name! },
                Organization = new OrganizationDto { Id = application.OrganizationId, Name = application.Organization?.Name! },
                JobTitle = new JobTitleDto { Id = application.JobTitleId, Name = application.JobTitle?.Name! },
                WorkEnvironment = new WorkEnvironmentDto { Id = application.WorkEnvironmentId, Name = application.WorkEnvironment?.Name! },
                Location = application.Location != null
                    ? new LocationDto { Id = application.LocationId, Name = application.Location?.Name! }
                    : null
            };
        }
    }
}
