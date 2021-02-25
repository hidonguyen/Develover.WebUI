using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Develover.WebUI.Identity;
using Develover.WebUI.Models;
using Microsoft.AspNetCore.Http;
using Develover.WebUI.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Develover.WebUI.Extensions;
using Develover.WebUI.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Develover.WebUI.Controllers
{
    public class SettingController : Controller
    {
        private readonly ILogger<SettingController> _logger;
        private readonly DataContext _context;
        private readonly IBranchManager _branchManager;

        public SettingController(ILogger<SettingController> logger, DataContext context, IBranchManager branchManager)
        {
            _logger = logger;
            _context = context;
            _branchManager = branchManager;
        }

        public IActionResult Index()
        {
            var model = _context.DeveloverSettings.FirstOrDefault();

            if (model == null)
                model = new DeveloverSetting();

            #region Source cho dropdownlist
            ViewData["Currencies"] = new List<SelectListItem>(_context.Currencies
                .OrderByDescending(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }));
            ViewData["Branchs"] = new List<SelectListItem>(_context.Branches
                .OrderByDescending(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }));
            #endregion
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserSettingsAsync()
        {
            _ = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);

            var userSettings = _context.UserSettings.Include(us => us.Setting).Where(us => us.UserId == userId);

            if (!userSettings.Any()) //Chưa có setting cho user
            {
                await _context.UserSettings.AddRangeAsync(_context.Settings.Select(s => new Entities.UserSetting { UserId = userId, SettingId = s.Id, Value = s.DefaultValue, Selector = s.DefaultSelector }));
                await _context.SaveChangesAsync();

                return Json(_context.Settings.Select(s => new UserSettingViewModel(userId, s.Id, s.Name, s.Description, s.DefaultValue, s.DefaultSelector, s.DefaultValue, s.DefaultSelector)));
            }

            return Json(userSettings.Select(us => new UserSettingViewModel(us.UserId, us.SettingId, us.Setting.Name, us.Setting.Description, us.Setting.DefaultValue, us.Setting.DefaultSelector, us.Value, us.Selector)));
        }

        [HttpPost]
        public async Task SetUserSetting(string id, string value, string selector)
        {
            _ = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            _ = Guid.TryParse(id, out Guid settingId);

            var userSetting = _context.UserSettings.FirstOrDefault(us => us.UserId == userId && us.SettingId == settingId);

            if (userSetting == null)
            {
                _context.UserSettings.Add(new Entities.UserSetting { UserId = userId, SettingId = settingId, Value = value ?? "", Selector = selector ?? "" });
            }
            else
            {
                userSetting.Value = value ?? "";
                userSetting.Selector = selector ?? "";

                _context.UserSettings.Update(userSetting);
            }

            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public IActionResult GetAllBranches()
        {
            return Json(_branchManager.GetAllBranches()
                .Select(b => new
                {
                    Value = b.Id.ToLowerString(),
                    Text = b.Name,
                    Selected = b.Id == _branchManager.GetCurrentBranchId()
                }));
        }

        [HttpGet]
        public IActionResult GetCurrentBranchName()
        {
            return Json(_branchManager.GetCurrentBranch()?.Name);
        }

        [HttpGet]
        public IActionResult GetPrimaryBranchId()
        {
            return Json(_branchManager.GetPrimaryBranchId().ToLowerString());
        }

        [HttpGet]
        public IActionResult GetCurrentBranchId()
        {
            return Json(_branchManager.GetCurrentBranchId().ToLowerString());
        }

        [HttpPost]
        public void SetCurrentBranchId(Guid branchId)
        {
            _branchManager.SetCurrentBranchId(branchId);
        }

        [HttpPost]
        public ActionResult SaveDeveloverSetting(DeveloverSetting model)
        {
            if (ModelState.IsValid)
            {
                if (model.DecimalSymbol == model.ThousandsSymbol)
                {
                    Response.StatusCode = 400;
                    ModelState.AddModelError("DecimalSymbol", "DecimalSymbol and ThousandsSymbol are the same");
                    return Json(ModelState.AllModels());
                }
                if (model.StructureNo == "")
                {
                    model.StructureNo = "{YYYY}/{MM}-{NO}";
                }
                if (model.LenghtNo == 0)
                {
                    model.LenghtNo = 3;
                }
                //var totalCharAllow = model.StructureNo.CountChar('{') +
                //    model.StructureNo.CountChar('}') +
                //    model.StructureNo.CountChar('Y') +
                //    model.StructureNo.CountChar('M') +
                //    model.StructureNo.CountChar('D') +
                //    model.StructureNo.CountChar('N') +
                //    model.StructureNo.CountChar('O') +
                //    model.StructureNo.CountChar('/') +
                //    model.StructureNo.CountChar('\\') +
                //    model.StructureNo.CountChar('-') +
                //    model.StructureNo.CountChar(' ');
                //if (totalCharAllow != model.StructureNo.Length) {
                //    Response.StatusCode = 400;
                //    ModelState.AddModelError("StructureNo", "StructureNo Contains disallowed characters.");
                //    return Json(ModelState.AllModels());
                //}

                _context.DeveloverSettings.Update(model);

                _context.SaveChanges();

                TempData["Notification"] = $"{model.GetType().Name} successfully saved";
                return Json(new { model.Id, ModelState = ModelState.AllModels() });
            }

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }

        [HttpGet]
        public async Task<ActionResult> GetDeveloverSettingAsync()
        {
            var develoverSetting = _context.DeveloverSettings.FirstOrDefault();
            if (develoverSetting != null)
            {
                var userSettings = await GetUserSettingsAsync();
                return Json(new { develoverSetting, userSettings });
            }

            Response.StatusCode = 400;
            return Json("");
        }
    }
}
