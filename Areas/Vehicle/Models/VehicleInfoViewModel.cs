using Develover.WebUI.Entities;
using System;
using System.Collections.Generic;

namespace Develover.WebUI.Areas.Vehicle.Models
{
    public class VehicleInfoViewModel
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
        public bool IsScheduled { get; set; }
        public bool IsRepairing { get; set; }
        public bool IsDriving { get; set; }
        public string IsScheduledText { get; set; }
        public string IsRepairingText { get; set; }
        public string IsDrivingText { get; set; }
        public string Note { get; set; }

    }
    public class VehicleStatus
    {
        public Guid Id { get; set; }
        public bool IsScheduled { get; set; }
        public bool IsRepairing { get; set; }
        public bool IsDriving { get; set; }
        public Guid? CurrentVehicleScheduleId { get; set; }

    }
}
