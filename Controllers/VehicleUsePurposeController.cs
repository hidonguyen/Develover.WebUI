﻿using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class VehicleUsePurposeController : Controller
    {
        private readonly ILogger<VehicleUsePurposeController> _logger;

        public VehicleUsePurposeController(ILogger<VehicleUsePurposeController> logger)
        {
            _logger = logger;
        }

        private VehicleUsePurposeViewModel model = new VehicleUsePurposeViewModel() { Id = Guid.NewGuid(), Name = "Test", Status = true };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<VehicleUsePurposeViewModel> result = new List<VehicleUsePurposeViewModel>
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
        public IActionResult Save(VehicleUsePurposeViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
