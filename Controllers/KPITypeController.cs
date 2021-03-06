﻿using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class KPITypeController : Controller
    {
        private readonly ILogger<KPITypeController> _logger;

        public KPITypeController(ILogger<KPITypeController> logger)
        {
            _logger = logger;
        }

        private KPITypeViewModel model = new KPITypeViewModel() { Id = Guid.NewGuid() };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<KPITypeViewModel> result = new List<KPITypeViewModel>
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
        public IActionResult Save(KPITypeViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
