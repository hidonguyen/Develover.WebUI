using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class GoodsReceiptNoteViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string No { get; set; }
        public Guid SupplierId { get; set; }
        public string Supplier { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalAmount { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
    }
    public class GoodsReceiptNoteItemViewModel
    {
        public Guid Id { get; set; }
        public Guid GoodsReceiptNoteId { get; set; }
        public Guid StockItemId { get; set; }
        public string StockItem { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public Guid LocationId { get; set; }
        public string Location { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
    }
}
