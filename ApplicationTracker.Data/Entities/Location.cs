using System.ComponentModel.DataAnnotations;

namespace ApplicationTracker.Data.Entities
{
#pragma warning disable CS8618
    public class Location : BaseEntity
    {
        public string? City { get; set; }
        [StringLength (50)]
        public string State { get; set; }
        [StringLength(50)]
        public string Country { get; set; } = "United States";
        public List<Application> Applications { get; set; }
    }
#pragma warning restore CS8618
}
