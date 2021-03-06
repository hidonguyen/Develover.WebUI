﻿using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class BranchesController : Controller
    {
        private readonly ILogger<BranchesController> _logger;

        public BranchesController(ILogger<BranchesController> logger)
        {
            _logger = logger;
        }

        private BranchViewModel model = new BranchViewModel() { Id = Guid.NewGuid(), Name = "Test", Status = true };

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<BranchViewModel> result = new List<BranchViewModel>
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
        public IActionResult Save(BranchViewModel model) => Ok();

        [HttpPost]
        public IActionResult Delete(string id) => Ok();
    }
}
