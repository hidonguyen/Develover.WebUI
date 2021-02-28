using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly ILogger<ChangePasswordController> _logger;

        public ChangePasswordController(ILogger<ChangePasswordController> logger)
        {
            _logger = logger;
        }

        private ChangePasswordViewModel model = new ChangePasswordViewModel() { Id = Guid.NewGuid(), Username = "admin" };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<ChangePasswordViewModel> result = new List<ChangePasswordViewModel>
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
        public IActionResult Save(ChangePasswordViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
