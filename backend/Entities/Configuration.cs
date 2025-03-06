using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Entities
{
    public class Configuration
    {
        public int Id { get; set; }
        public required string Key { get; set; }
        public required string Value { get; set; }
        public bool IsActive { get; set; }
    }
}