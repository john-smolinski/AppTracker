using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services;
using ApplicationTracker.Services.Factory;
using ApplicationTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ApplicationTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("Tracker");
            builder.Services.AddDbContext<TrackerDbContext>(options => 
                options.UseSqlServer(connectionString));

            // factory for creating service instances
            builder.Services.AddScoped<ServiceFactory>();

            // add services 
            builder.Services.AddScoped<JobTitleService>();
            builder.Services.AddScoped<IService<JobTitleDto>, JobTitleService>();

            builder.Services.AddScoped<LocationService>();
            builder.Services.AddScoped<IService<LocationDto>, LocationService>();

            builder.Services.AddScoped<OrganizationService>();
            builder.Services.AddScoped<IService<OrganizationDto>, OrganizationService>();

            builder.Services.AddScoped<SourceService>();
            builder.Services.AddScoped<IService<SourceDto>, SourceService>();

            builder.Services.AddScoped<WorkEnvironmentService>();
            builder.Services.AddScoped<IService<WorkEnvironmentDto>, WorkEnvironmentService>();

            builder.Services.AddScoped<IApplicationService<ApplicationDto>, ApplicationService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Host.UseSerilog((context, config) =>
                config.ReadFrom.Configuration(context.Configuration));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
