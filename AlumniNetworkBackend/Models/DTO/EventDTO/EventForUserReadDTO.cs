using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniNetworkBackend.Models.DTO.EventDTO
{
    public class EventForUserReadDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
