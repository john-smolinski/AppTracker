using ApplicationTracker.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Data.Entities
{
#pragma warning disable CS8618
    public class AppEvent 
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public Application Application { get; set; }
        public DateTime EventDate { get; set; }
        [StringLength(50)]
        public ContactMethod ContactMethod { get; set; }
        [StringLength(50)]
        public EventType EventType { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
#pragma warning restore CS8618
}
