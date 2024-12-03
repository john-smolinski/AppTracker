using System.ComponentModel.DataAnnotations;

namespace ApplicationTracker.Data.Entities
{
#pragma warning disable CS8618
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
    }
#pragma warning restore CS8618
}
