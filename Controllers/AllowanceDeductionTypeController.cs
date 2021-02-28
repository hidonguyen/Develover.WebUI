using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class AllowanceDeductionTypeController : Controller
    {
        private readonly ILogger<AllowanceDeductionTypeController> _logger;

        public AllowanceDeductionTypeController(ILogger<AllowanceDeductionTypeController> logger)
        {
            _logger = logger;
        }

        private AllowanceDeductionTypeViewModel model = new AllowanceDeductionTypeViewModel() { Id = Guid.NewGuid(), Name = "Test", Status = true };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<AllowanceDeductionTypeViewModel> result = new List<AllowanceDeductionTypeViewModel>
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
        public IActionResult Save(AllowanceDeductionTypeViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
