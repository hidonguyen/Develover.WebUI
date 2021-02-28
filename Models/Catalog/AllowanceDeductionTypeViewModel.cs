using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class AllowanceDeductionTypeViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool ShowOnPayslip { get; set; }
        public bool IsDeduction { get; set; }
        public bool Status { get; set; }
    }
}
