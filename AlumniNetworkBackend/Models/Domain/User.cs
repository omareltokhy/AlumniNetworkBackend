﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniNetworkBackend.Models.Domain
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(40)]
        public string Picture { get; set; }
        public string Status { get; set; }
        [MaxLength(100)]
        public string Bio { get; set; }
        [MaxLength(200)]
        public string FunFact { get; set; }
    }
}
