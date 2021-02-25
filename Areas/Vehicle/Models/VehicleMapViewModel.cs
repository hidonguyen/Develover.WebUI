using System;
using System.Collections.Generic;

namespace Develover.WebUI.Areas.Vehicle.Models
{
    public class VehicleMapViewModel
    {
        public List<VehicleViewModel> Vehicles { get; set; }
        public VehicleInfoViewModel VehicleInfo { get; set; }

        public VehicleMapViewModel()
        {
            VehicleInfo = new VehicleInfoViewModel();
        }

    }
}
