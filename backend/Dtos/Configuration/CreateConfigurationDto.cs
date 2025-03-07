using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.Configuration
{
    public class CreateConfigurationDto
    {
        public required string Key { get; set; }
        public required string Value { get; set; }

    }
}