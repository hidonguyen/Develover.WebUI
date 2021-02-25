using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Areas.Stationery.Models
{
    public class OpeningStockViewModel
    {
        public Guid Id { get; set; }
        public DateTime DateUpdate { get; set; }
        public double Quantity { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
    }
    public class OpeningStockItemViewModel
    {
        public Guid Id { get; set; }
        public Guid OpeningStockId { get; set; }
        public string OpeningStock { get; set; }
        public Guid LocationId { get; set; }
        public string Location { get; set; }
        public Guid StockItemId { get; set; }
        public string StockItem { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public double Quantity { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
    }
}