using ApplicationTracker.Data.Dtos;

namespace ApplicationTracker.Services
{
    public interface IService<T>
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        IEnumerable<ApplicationDto> GetApplications(int id);
    }
}
