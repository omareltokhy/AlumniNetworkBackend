using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniNetworkBackend.Models.DTO
{
    public class UserCreateDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Picture { get; set; }
        public string Status { get; set; } 
        public string Bio { get; set; }
        public string FunFact { get; set; }
    }
}
