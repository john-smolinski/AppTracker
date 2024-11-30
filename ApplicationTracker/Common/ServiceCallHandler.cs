using ApplicationTracker.Services.Factory;
using ApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationTracker.Common
{
    public static class ServiceCallHandler
    {
        public static async Task<ActionResult> HandleServiceCall<TService>(
                ServiceFactory serviceFactory,
                ILogger logger,
                Func<IService<TService>, Task<ActionResult>> action)
            where TService : class
        {
            try
            {
                var service = serviceFactory.GetService<TService>();
                return await action(service);
            }
            catch (InvalidOperationException ioe)
            {
                logger.LogError(ioe, "Service not found for type {TypeName}", typeof(TService).Name);
                return ErrorHelper.InternalServerError("Service not found", ioe.Message);
            }
            catch (ArgumentException ae)
            {
                logger.LogError(ae, "Service not registered in ServiceFactory for type {TypeName}", typeof(TService).Name);
                return ErrorHelper.InternalServerError("Service not registered in ServiceFactory", ae.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred while processing {TypeName}", typeof(TService).Name);
                return ErrorHelper.InternalServerError("Unhandled exception occurred", ex.Message);
            }
        }
    }
}
