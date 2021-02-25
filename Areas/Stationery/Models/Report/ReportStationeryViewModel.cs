using Develover.WebUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Areas.Stationery.Models
{
    public class ReportStationeryViewModel
    {
        public string DataUrl { get; set; }
        public List<ColumnBootstrapTable> Columns { get; set; }
    }
}
