using System.ComponentModel.DataAnnotations;

namespace ApplicationTracker.Data.Entities
{
#pragma warning disable CS8618
    public class WorkEnvironment
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public List<Application> Applications { get; set; }
    }
#pragma warning restore CS8618
}
