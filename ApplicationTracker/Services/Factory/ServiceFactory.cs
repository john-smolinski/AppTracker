using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services.Interfaces;

namespace ApplicationTracker.Services.Factory
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, Type> _serviceMap;

        /// <summary>
        /// Factory for getting a service related to the Dto passed
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider</param>
        public ServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _serviceMap = new Dictionary<Type, Type>
            {
                { typeof(JobTitleDto), typeof(JobTitleService) },
                { typeof(LocationDto), typeof(LocationService) },
                { typeof(OrganizationDto), typeof(OrganizationService) },
                { typeof(SourceDto), typeof(SourceService) },
                { typeof(WorkEnvironmentDto), typeof(WorkEnvironmentService) }
            };
        }

        /// <summary>
        /// Get the service related to Dto passed
        /// </summary>
        /// <typeparam name="T">Dto class to get service for</typeparam>
        /// <returns>IService<T></returns>
        /// <exception cref="InvalidOperationException">Thrown when no service for type is found</exception>
        /// <exception cref="ArgumentException">Thrown when no matching service found in map</exception>
        public IService<T> GetService<T>()
            where T : class
        {
            if (_serviceMap.TryGetValue(typeof(T), out var serviceType))
            {
                if (_serviceProvider.GetService(serviceType) is not IService<T> service)
                {
                    throw new InvalidOperationException($"No service found for type {serviceType.Name}.");
                }
                return service;
            }
            throw new ArgumentException($"No service registered for type {typeof(T).Name}");
        }
    }
}
