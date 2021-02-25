using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class VehicleRepairViewModel
    {
        public Guid Id { get; set; }
        public DateTime IssueDate { get; set; }
        public string No { get; set; }
        public int SequenceNo { get; set; }
        public Guid VehicleId { get; set; }
        public string Vehicle { get; set; }
        public Guid DriverId { get; set; }
        public string Driver { get; set; }
        public string RepairShop { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
        public double EstimatedRepairCost { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalAmount { get; set; }
        public string Note { get; set; }
    }
    public class VehicleRepairItemViewModel
    {
        public Guid Id { get; set; }
        public Guid VehicleRepairId { get; set; }
        public Guid VehicleCostId { get; set; }
        public string VehicleCost { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
    }
}
