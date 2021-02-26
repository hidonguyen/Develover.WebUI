using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Models
{
    public class VehicleViewModel
    {
        public Guid Id { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string EngineNo { get; set; }
        public string ChassisNo { get; set; }
        public string Brand { get; set; }
        public string ModelNo { get; set; }
        public string Color { get; set; }
        public int Capacity { get; set; }
        public Guid? DefaultDriverId { get; set; }
        public string DefaultDriver { get; set; }
        public Guid? DefaultDepartmentId { get; set; }
        public string DefaultDepartment { get; set; }
        public Guid? CurrentVehicleScheduleId { get; set; }
        public Guid? CurrentVehicleRepairId { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
    }
}
