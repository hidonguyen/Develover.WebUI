using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class DepartmentTransferController : Controller
    {
        private readonly ILogger<DepartmentTransferController> _logger;

        public DepartmentTransferController(ILogger<DepartmentTransferController> logger)
        {
            _logger = logger;
        }

        private DepartmentTransferViewModel model = new DepartmentTransferViewModel() { Id = Guid.NewGuid(), Status = true };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<DepartmentTransferViewModel> result = new List<DepartmentTransferViewModel>
            {
                model
            };

            return Json(new { data = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetModel(string id)
        {

            return Json(new { model });
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult New() => View(nameof(Detail));

        [HttpGet]
        public IActionResult Detail(string id = "") => View();

        [HttpPost]
        public IActionResult Save(DepartmentTransferViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
