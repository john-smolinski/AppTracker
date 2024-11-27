namespace ApplicationTracker.Data.Dtos
{
#pragma warning disable CS8618
    public class ApplicationDto
    {
        public DateOnly ApplicaitionDate { get; set; }
        public SourceDto Source { get; set; }
        public OrganizationDto Organization { get; set; }
        public JobTitleDto JobTitle { get; set; }
        public WorkEnvironmentDto WorkEnvironment { get; set; }
        public LocationDto Location { get; set; }
    }
#pragma warning restore CS8618
}
