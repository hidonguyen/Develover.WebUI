using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class KPITypeViewModel
    {
        public Guid Id { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Value0 { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
    }
}
