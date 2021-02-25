using System;

namespace Develover.WebUI.Areas.Vehicle.Models
{
    public class VehicleViewModel
    {
        public Guid Id { get; set; }
        public string RegistrationNo { get; set; }
        public string EngineNo { get; set; }

        public Guid? DriverId { get; set; }
        public string DriverName { get; set; }

        public bool IsScheduled { get; set; }
        public Guid? VehicleScheduleId { get; set; }

        public bool IsDriving { get; set; }

        public bool IsRepairing { get; set; }
        public Guid? VehicleRepairId { get; set; }

    }
}
