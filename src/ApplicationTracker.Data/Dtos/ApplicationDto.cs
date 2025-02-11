namespace ApplicationTracker.Data.Dtos
{
#pragma warning disable CS8618
    public class ApplicationDto
    {
        public int? Id { get; set; }
        public DateOnly ApplicationDate { get; set; }
        public SourceDto Source { get; set; }
        public OrganizationDto Organization { get; set; }
        public JobTitleDto JobTitle { get; set; }
        public WorkEnvironmentDto WorkEnvironment { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }
#pragma warning restore CS8618
}
