using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class DivisionViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Guid DepartmentId { get; set; }
        public string Department { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
    }
}
