namespace ApplicationTracker.Data.Entities
{
#pragma warning disable CS8618
    public class Application
    {
        public int Id { get; set; }
        public DateOnly ApplicationDate {  get; set; }
        public int SourceId { get; set; }
        public Source Source { get; set; }
        public int OrganizationId {  get; set; }
        public Organization Organization { get; set; }
        public int JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }
        public int EnvironmentId { get; set; }
        public WorkEnvironment Environment { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
#pragma warning restore CS8618
}
