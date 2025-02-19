using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTracker.Data.Entities
{
#pragma warning disable CS8618
    [ExcludeFromCodeCoverage]
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
    }
#pragma warning restore CS8618
}
