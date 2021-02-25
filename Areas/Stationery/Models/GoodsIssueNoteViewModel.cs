using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class GoodsIssueNoteViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string No { get; set; }
        public Guid DepartmentId { get; set; }
        public string Department { get; set; }
        public Guid ReceiverId { get; set; }
        public string Receiver { get; set; }
        public double TotalQuantity { get; set; }
        public string Note { get; set; }
    }
    public class GoodsIssueNoteItemViewModel
    {
        public Guid Id { get; set; }
        public Guid GoodsIssueNoteId { get; set; }
        public Guid StockItemId { get; set; }
        public string StockItem { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public Guid LocationId { get; set; }
        public string Location { get; set; }
        public double Quantity { get; set; }
        public string Note { get; set; }
    }
}
