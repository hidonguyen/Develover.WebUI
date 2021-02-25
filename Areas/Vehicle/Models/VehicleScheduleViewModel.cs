using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class VehicleScheduleViewModel
    {
        public Guid Id { get; set; }
        public DateTime IssueDate { get; set; }
        public string No { get; set; }
        public int SequenceNo { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public Guid VehicleId { get; set; }
        public string Vehicle { get; set; }
        public Guid DriverId { get; set; }
        public string Driver { get; set; }
        public Guid? PetitionerId { get; set; }
        public string Petitioner { get; set; }
        public Guid? DepartmentId { get; set; }
        public string Department { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public Guid VehicleUsePurposeId { get; set; }
        public string UsePurpose { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalAmount { get; set; }
        public string Note { get; set; }
    }
    public class VehicleScheduleItemViewModel
    {
        public Guid Id { get; set; }
        public Guid VehicleScheduleId { get; set; }
        public DateTime IssueDate { get; set; }
        public Guid VehicleCostId { get; set; }
        public string VehicleCost { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
    }
}
