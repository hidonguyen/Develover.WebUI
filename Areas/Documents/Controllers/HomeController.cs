using Develover.WebUI.Areas.Documents.Models;
using Develover.WebUI.DbContexts;
using Develover.WebUI.Entities;
using Develover.WebUI.Extensions;
using Develover.WebUI.Helpers;
using Develover.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Areas.Documents.Controllers
{
    [Area("Documents")]
    [Route("documents/{controller=home}/{action=index}")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
