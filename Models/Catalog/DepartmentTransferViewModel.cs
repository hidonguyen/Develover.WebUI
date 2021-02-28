using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class DepartmentTransferViewModel
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid DivisionId { get; set; }
        public string No { get; set; }
        public string EffectiveDate { get; set; }
        public string Note { get; set; }

        public bool Status { get; set; }
    }
}
