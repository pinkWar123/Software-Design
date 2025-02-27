using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Entities
{
    public class StatusTransition
    {
        public int SourceStatusId { get; set; }
        public int TargetStatusId { get; set; }
        public Status SourceStatus { get; set; }
        public Status TargetStatus { get; set; }
    }
}