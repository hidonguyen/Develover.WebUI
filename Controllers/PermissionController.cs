﻿using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class PermissionController : Controller
    {
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(ILogger<PermissionController> logger)
        {
            _logger = logger;
        }

        private PermissionViewModel model = new PermissionViewModel() { Id = Guid.NewGuid(), Name = "Test", Status = true };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<PermissionViewModel> result = new List<PermissionViewModel>
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
        public IActionResult Save(PermissionViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
