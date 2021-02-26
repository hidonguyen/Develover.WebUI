using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(ILogger<SupplierController> logger)
        {
            _logger = logger;
        }

        private SupplierViewModel model = new SupplierViewModel() { Id = Guid.NewGuid(), Name = "Test", Status = true };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<SupplierViewModel> result = new List<SupplierViewModel>
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
        public IActionResult Save(SupplierViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
