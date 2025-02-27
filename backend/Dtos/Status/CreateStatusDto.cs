using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.Status
{
    public class CreateStatusDto
    {
        public string Name { get; set; }
        public List<int> OutgoingTransitions { get; set; }
    }
}