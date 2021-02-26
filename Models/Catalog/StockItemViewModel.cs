using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class StockItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public Guid? DefaultLocationId { get; set; }
        public string DefaultLocation { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
    }
}
