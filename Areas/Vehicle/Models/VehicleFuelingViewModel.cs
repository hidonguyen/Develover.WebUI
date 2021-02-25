using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class VehicleFuelingViewModel
    {
        public Guid Id { get; set; }
        public DateTime IssueDate { get; set; }
        public string No { get; set; }
        public Guid IssuerId { get; set; }
        public string Issuer { get; set; }
        public double TotalQuantity { get; set; }
        public string Note { get; set; }
    }
    public class VehicleFuelingItemViewModel
    {
        public Guid Id { get; set; }
        public Guid VehicleFuelingId { get; set; }
        public Guid VehicleId { get; set; }
        public string Vehicle { get; set; }
        public Guid ReceiverId { get; set; }
        public string Receiver { get; set; }
        public double Quantity { get; set; }
        public double CurrentKM { get; set; }
        public string Note { get; set; }
    }
}
