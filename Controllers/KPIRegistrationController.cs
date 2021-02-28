using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class KPIRegistrationController : Controller
    {
        private readonly ILogger<KPIRegistrationController> _logger;

        public KPIRegistrationController(ILogger<KPIRegistrationController> logger)
        {
            _logger = logger;
        }

        private KPIRegistrationViewModel model = new KPIRegistrationViewModel() { Id = Guid.NewGuid(), KPIType = "Test" };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<KPIRegistrationViewModel> result = new List<KPIRegistrationViewModel>
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
        public IActionResult Save(KPIRegistrationViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
