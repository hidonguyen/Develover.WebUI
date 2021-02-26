using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class DocumentStatusController : Controller
    {
        private readonly ILogger<DocumentStatusController> _logger;

        public DocumentStatusController(ILogger<DocumentStatusController> logger)
        {
            _logger = logger;
        }

        private DocumentStatusViewModel model = new DocumentStatusViewModel() { Id = Guid.NewGuid(), Name = "Test", Status = true };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<DocumentStatusViewModel> result = new List<DocumentStatusViewModel>
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
        public IActionResult Save(DocumentStatusViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
