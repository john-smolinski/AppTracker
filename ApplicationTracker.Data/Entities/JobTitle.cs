﻿using System.ComponentModel.DataAnnotations;

namespace ApplicationTracker.Data.Entities
{
#pragma warning disable CS8618
    public class JobTitle : BaseEntity
    {
        public List<Application> Applications { get; set; }
    }
#pragma warning restore CS8618
}