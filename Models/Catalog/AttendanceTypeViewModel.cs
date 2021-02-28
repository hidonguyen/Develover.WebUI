using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class AttendanceTypeViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Multiplier { get; set; }
        public string HasMealAllowance { get; set; }
        public string IsAnnualLeave { get; set; }
        public bool Status { get; set; }
    }
}
