﻿using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class DocumentCategoryController : Controller
    {
        private readonly ILogger<DocumentCategoryController> _logger;

        public DocumentCategoryController(ILogger<DocumentCategoryController> logger)
        {
            _logger = logger;
        }

        private DocumentCategoryViewModel model = new DocumentCategoryViewModel() { Id = Guid.NewGuid(), Name = "Test", Status = true };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<DocumentCategoryViewModel> result = new List<DocumentCategoryViewModel>
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
        public IActionResult Save(DocumentCategoryViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
