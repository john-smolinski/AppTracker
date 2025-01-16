namespace ApplicationTracker.Services.Interfaces
{
    public interface IServiceFactory
    {
        IService<T> GetService<T>() where T : class;
    }
}
