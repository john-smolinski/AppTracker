using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Data.Dtos
{
#pragma warning disable CS8618
    public class AppEventDto
    {
        public int? Id { get; set; }
        public int ApplicationId { get; set; }
        public DateTime EventDate { get; set; }
        public string ContactMethod { get; set; }
        public string EventType { get; set; }
        public string? Description { get; set; }

    }
#pragma warning restore CS8618
}
