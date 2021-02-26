using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class VehicleUsePurposeViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
    }
}
