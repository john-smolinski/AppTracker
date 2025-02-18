using ApplicationTracker.Data.Dtos;
using System.Threading.Tasks;

namespace ApplicationTracker.Services.Interfaces
{
    public interface IApplicationService<ApplicationDto>
    {
        // Application methods
        Task<IEnumerable<ApplicationDto>> GetAllAsync();
        Task<ApplicationDto?> GetByIdAsync(int id);
        Task<ApplicationDto> PostAsync(ApplicationDto application);
        Task<ApplicationDto?> UpdateAsync(ApplicationDto application);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsAsync(ApplicationDto application);

        // AppEvent methods
        Task<IEnumerable<AppEventDto>> GetEventsAsync(int applicationId);
        Task<AppEventDto?> GetEventByIdAsync(int applicationId, int eventId);
        Task<AppEventDto> PostEventAsync(int applicationId, AppEventDto appEvent);
        Task<AppEventDto?> UpdateEventAsync(int applicationId, AppEventDto appEvent);
        Task<bool> EventExistsAsync(int applicationId, int eventId);
        Task<bool> DeleteEventAsync(int eventId);

    }
}
