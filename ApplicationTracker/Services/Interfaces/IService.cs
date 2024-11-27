using ApplicationTracker.Data.Dtos;

namespace ApplicationTracker.Services.Interfaces
{
    public interface IService<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<ApplicationDto>?> GetReleatedApplicationsAsync(int id);
    }
}
