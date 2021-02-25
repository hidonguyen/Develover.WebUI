using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class BranchViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Initial { get; set; }
        public string TaxNo { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNameE { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Director { get; set; }
        public string ChiefAccountant { get; set; }
        public string Treasurer { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool Status { get; set; }
    }
}
