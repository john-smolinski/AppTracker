using ApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationTracker.Common
{
    public static class ServiceCallHandler
    {
        public static async Task<ActionResult> HandleServiceCall<TService>(
                IServiceFactory serviceFactory,
                ILogger logger,
                Func<IService<TService>, Task<ActionResult>> action)
            where TService : class
        {
            if(action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                var service = serviceFactory.GetService<TService>();
                var result = await action(service);

                if(result == null)
                {
                    logger.LogError("The service call returned a null ActionResult for type {TypeName}", typeof(TService).Name);
                    return ErrorHelper.InternalServerError("Unexpected error", "The service call returned a null result.");
                }

                logger.LogDebug("Service call result: {ResultType}", result.GetType().Name);
                return result;
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
