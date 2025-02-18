using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Data.Enums;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApplicationTracker.Services
{
    public class ApplicationService : IApplicationService<ApplicationDto>
    {
        private readonly TrackerDbContext _context;
        private readonly ILogger<ApplicationService> _logger;
        
        public ApplicationService(TrackerDbContext context, ILogger<ApplicationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ApplicationDto>> GetAllAsync()
        {
            try
            {

                var applications = await _context.Applications
                            .AsNoTracking()
                            .Include(a => a.Source)
                            .Include(a => a.Organization)
                            .Include(a => a.JobTitle)
                            .Include(a => a.WorkEnvironment)
                            .Select(a => new
                            {
                                Application = a,
                                HasRejectionEvent = _context.AppEvents
                                    .Any(e => e.ApplicationId == a.Id && e.EventType == EventType.Rejection)
                            })
                            .ToListAsync();

                return applications.Select(a => MapToDto(a.Application, a.HasRejectionEvent));
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
                var application = await _context.Applications
                            .AsNoTracking()
                            .Include(a => a.Source)
                            .Include(a => a.Organization)
                            .Include(a => a.JobTitle)
                            .Include(a => a.WorkEnvironment)
                            .Include(a => a.AppEvents)
                            .Where(a => a.Id == id)
                            .Select(a => new
                            {
                                Application = a,
                                HasRejectionEvent = _context.AppEvents
                                    .Any(e => e.ApplicationId == a.Id && e.EventType == EventType.Rejection)
                            })
                            .FirstOrDefaultAsync();

                return application != null ? MapToDto(application.Application, application.HasRejectionEvent) : null;
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
                // prevent dupicate/double posts
                if(await ExistsAsync(application))
                {
                    throw new InvalidDataException("Application already exists.");
                }

                var source = await AddEntity<Source>(application.Source);
                _context.Attach(source);

                var organization = await AddEntity<Organization>(application.Organization);
                _context.Attach(organization);

                var jobTitle = await AddEntity<JobTitle>(application.JobTitle);
                _context.Attach(jobTitle);

                var workEnvironment = await AddEntity<WorkEnvironment>(application.WorkEnvironment);
                _context.Attach(workEnvironment);

                var newApplication = new Application
                {
                    // test to see if date was supplied. if not *assume* the date is today
                    ApplicationDate = application.ApplicationDate == default(DateOnly)
                        ? DateOnly.FromDateTime(DateTime.Now)
                        : application.ApplicationDate,
                    
                    Source = source,
                    Organization = organization,
                    JobTitle = jobTitle,
                    WorkEnvironment = workEnvironment,
                    City = application.City,
                    State = application.State
                };
                _context.Applications.Add(newApplication);
                await _context.SaveChangesAsync();

                return MapToDto(newApplication, false);
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
                _logger.LogError(ex, "An error occurred while checking existence of application with Id {Id}.", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(ApplicationDto application)
        {
            // ensure required relationships exist first
            var sourceId = await _context.Sources
                .AsNoTracking()
                .Where(x => x.Name == application.Source.Name)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var organizationId = await _context.Organizations
                .AsNoTracking()
                .Where(x => x.Name == application.Organization.Name)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var jobTitleId = await _context.JobTitles
                .AsNoTracking()
                .Where(x => x.Name == application.JobTitle.Name)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var workEnvironmentId = await _context.WorkEnvironments
                .AsNoTracking()
                .Where(x => x.Name == application.WorkEnvironment.Name)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            // check if any required relationship is missing
            if (sourceId == 0 || organizationId == 0 || jobTitleId == 0 || workEnvironmentId == 0)
            {
                return false;
            }

            // check if the application already exists
            var exists = await _context.Applications
                .AsNoTracking()
                .AnyAsync(x => x.SourceId == sourceId &&
                               x.OrganizationId == organizationId &&
                               x.JobTitleId == jobTitleId &&
                               x.WorkEnvironmentId == workEnvironmentId &&
                               x.ApplicationDate == application.ApplicationDate);

            return exists;
        }

        public async Task<ApplicationDto?> UpdateAsync(ApplicationDto application)
        {
            // validate required fields 
            if (application.Id == null) 
                throw new InvalidOperationException("Application Id cannot be null.");
            if (application.Source?.Id == null) 
                throw new InvalidOperationException("Source Id cannot be null.");
            if (application.Organization?.Id == null) 
                throw new InvalidOperationException("Organization Id cannot be null.");
            if (application.JobTitle?.Id == null) 
                throw new InvalidOperationException("Job Title Id cannot be null.");
            if (application.WorkEnvironment?.Id == null) 
                throw new InvalidOperationException("Work Environment Id cannot be null.");
            
            var app = await _context.Applications.FirstOrDefaultAsync(x => x.Id == application.Id.Value);

            if (app == null)
            {
                _logger.LogWarning("Application with Id {Id} not found", application.Id);
                throw new KeyNotFoundException($"Application with Id {application.Id} not found.");
            }

            app.ApplicationDate = application.ApplicationDate;
            app.SourceId = application.Source.Id.Value;
            app.OrganizationId = application.Organization.Id.Value;
            app.JobTitleId = application.JobTitle.Id.Value;
            app.WorkEnvironmentId = application.WorkEnvironment.Id.Value;
            app.City = application.City;
            app.State = application.State;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating application with Id {Id}.", application.Id);
                throw;
            }
            
            return await GetByIdAsync(application.Id.Value);
        }

        private static ApplicationDto MapToDto(Application application, bool hasRejectEvent)
        {
            return new ApplicationDto
            {
                Id = application.Id,
                ApplicationDate = application.ApplicationDate,
                Source = new SourceDto 
                { 
                    Id = application.SourceId, Name = application.Source?.Name! 
                },
                Organization = new OrganizationDto 
                { 
                    Id = application.OrganizationId, Name = application.Organization?.Name! 
                },
                JobTitle = new JobTitleDto 
                { 
                    Id = application.JobTitleId, Name = application.JobTitle?.Name! 
                },
                WorkEnvironment = new WorkEnvironmentDto 
                { 
                    Id = application.WorkEnvironmentId, Name = application.WorkEnvironment?.Name!
                },
                City = application.City,
                State = application.State,
                IsRejected = hasRejectEvent
            };
        }

        public async Task<IEnumerable<AppEventDto>> GetEventsAsync(int applicationId)
        {
            try
            {
                return await _context.AppEvents
                    .AsNoTracking()
                    .Where(x => x.ApplicationId == applicationId)
                    .Select(x => new AppEventDto
                    {
                        Id = x.Id,
                        ApplicationId = x.ApplicationId,
                        EventDate = x.EventDate,
                        ContactMethod = x.ContactMethod.ToString(),
                        EventType = x.EventType.ToString(),
                        Description = x.Description
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all events for application with Id {applicationId}.", applicationId);
                throw;
            }
        }

        public async Task<AppEventDto?> GetEventByIdAsync(int applicationId, int eventId)
        {
            try
            {
                return await _context.AppEvents
                    .AsNoTracking()
                    .Where(x => x.ApplicationId == applicationId && x.Id == eventId)
                    .Select(x => new AppEventDto
                    {
                        Id = x.Id,
                        ApplicationId = x.ApplicationId,
                        EventDate = x.EventDate,
                        ContactMethod = x.ContactMethod.ToString(),
                        EventType = x.EventType.ToString(),
                        Description = x.Description
                    }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching event with Id {eventId} for application with Id {applicationId}.", eventId, applicationId);
                throw;
            }
        }

        public async Task<AppEventDto> PostEventAsync(int applicationId, AppEventDto appEvent)
        {
            try
            {
                if(!await ExistsAsync(applicationId))
                {
                    throw new KeyNotFoundException($"Application with Id {applicationId} not found.");
                }

                if(!Enum.TryParse<ContactMethod>(appEvent.ContactMethod, out _)) 
                {
                    throw new InvalidEnumArgumentException($"Invalid ContactMethod. Expected values: {string.Join(", ", Enum.GetNames(typeof(ContactMethod)))}");
                }
                if(!Enum.TryParse<EventType>(appEvent.EventType, out _))
                {
                    throw new InvalidEnumArgumentException($"Invalid EventType. Expected values: {string.Join(", ", Enum.GetNames(typeof(EventType)))}");
                }

                var newEvent = new AppEvent
                {
                    ApplicationId = applicationId,
                    EventDate = appEvent.EventDate,
                    ContactMethod = Enum.Parse<ContactMethod>(appEvent.ContactMethod),
                    EventType = Enum.Parse<EventType>(appEvent.EventType),
                    Description = appEvent.Description
                };
                _context.AppEvents.Add(newEvent);
                await _context.SaveChangesAsync();

                return MapToDto(newEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new event for application with Id {applicationId}.", applicationId);
                throw;
            }
        }

        public async Task<AppEventDto?> UpdateEventAsync(int applicationId, AppEventDto appEvent)
        {
            // validate required fields
            if (appEvent.Id == null) 
                throw new InvalidOperationException("Event Id cannot be null.");
            if(appEvent.ApplicationId != applicationId) 
                throw new InvalidOperationException("Event does not belong to the specified application.");
            if(appEvent.EventDate == default) 
                throw new InvalidOperationException("Event Date cannot be null.");
            
            if(!Enum.TryParse<ContactMethod>(appEvent.ContactMethod, out _)) 
                throw new InvalidEnumArgumentException($"Invalid ContactMethod. Expected values: {string.Join(", ", Enum.GetNames(typeof(ContactMethod)))}");
            if(!Enum.TryParse<EventType>(appEvent.EventType, out _)) 
                throw new InvalidEnumArgumentException($"Invalid EventType. Expected values: {string.Join(", ", Enum.GetNames(typeof(EventType)))}");

            var app = await _context.AppEvents.FirstOrDefaultAsync(x => x.Id == appEvent.Id.Value);

            if (app == null)
            {
                _logger.LogWarning("AppEvent with Id {Id} not found", appEvent.Id);
                throw new KeyNotFoundException($"AppEvent with Id {appEvent.Id} not found.");
            }

            app.EventDate = appEvent.EventDate;
            app.ContactMethod = Enum.Parse<ContactMethod>(appEvent.ContactMethod);
            app.EventType = Enum.Parse<EventType>(appEvent.EventType);
            app.Description = appEvent.Description;

            try
            {   
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating event with Id {Id}.", appEvent.Id);
                throw;
            }

            return await GetEventByIdAsync(applicationId, appEvent.Id.Value);
        }

        public Task<bool> EventExistsAsync(int applicationId, int eventId)
        {
            try
            {
                return _context.AppEvents.AnyAsync(x => x.ApplicationId == applicationId && x.Id == eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking existence of event with Id {eventId} for application with Id {applicationId}.", eventId, applicationId);
                throw;
            }
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var appEvent = await _context.AppEvents.FindAsync(eventId);

            if (appEvent == null)
            {
                throw new KeyNotFoundException($"AppEvent with Id {eventId} not found.");
            }

            _context.AppEvents.Remove(appEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        private AppEventDto MapToDto(AppEvent appEvent)
        {
            return new AppEventDto
            {
                Id = appEvent.Id,
                ApplicationId = appEvent.ApplicationId,
                EventDate = appEvent.EventDate,
                ContactMethod = appEvent.ContactMethod.ToString(),
                EventType = appEvent.EventType.ToString(),
                Description = appEvent.Description
            };
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding or retrieving an entity of type {EntityType}.", typeof(T).Name);
                throw;
            }
        }
    }
}
