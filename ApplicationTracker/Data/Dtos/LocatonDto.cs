﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApplicationTracker.Data.Dtos
{
#pragma warning disable CS8618
    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string Country { get; set; }
    }    
#pragma warning restore CS8618
}