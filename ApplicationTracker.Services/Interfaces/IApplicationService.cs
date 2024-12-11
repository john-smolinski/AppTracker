using ApplicationTracker.Data.Dtos;
using System.Threading.Tasks;

namespace ApplicationTracker.Services.Interfaces
{
    public interface IApplicationService<ApplicationDto>
    {
        Task<IEnumerable<ApplicationDto>> GetAllAsync();
        Task<ApplicationDto?> GetByIdAsync(int id);
        Task<ApplicationDto> PostAsync(ApplicationDto application);
        Task<bool> ExistsAsync(int id);
        // TODO:
        //Task<ApplicationDto> PutAsync(ApplicationDto application);
    }
}
