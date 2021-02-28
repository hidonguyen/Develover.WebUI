using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class KPIRegistrationViewModel
    {
        public Guid Id { get; set; }
        public string DivisionId { get; set; }
        public string KPIType { get; set; }
        public DateTime IssueDate { get; set; }
    }
}
