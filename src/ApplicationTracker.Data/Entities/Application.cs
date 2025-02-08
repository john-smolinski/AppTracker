using System.ComponentModel.DataAnnotations;

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
        public int WorkEnvironmentId { get; set; }
        public WorkEnvironment WorkEnvironment { get; set; }
        [StringLength(50)]
        public string? City { get; set; }
        [StringLength(2)]
        public string? State { get; set; }
    }
#pragma warning restore CS8618
}
