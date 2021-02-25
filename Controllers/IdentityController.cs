using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Develover.WebUI.DbContexts;
using Develover.WebUI.Entities;
using Develover.WebUI.Identity;
using Develover.WebUI.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Develover.WebUI.Extensions;

namespace Develover.WebUI.Controllers
{
    public class IdentityController : Controller
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IUserManager _userManager;
        private readonly DataContext _context;

        public IdentityController(ILogger<IdentityController> logger, IUserManager userManager, DataContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            LoginViewModel model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ValidateResult validateResult = _userManager.Validate(model.Username, model.Password);

                if (validateResult.Success)
                {
                    await _userManager.SignInAsync(HttpContext, validateResult.User, model.RememberMe);

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    switch (validateResult.Error)
                    {
                        case ValidateResultError.CredentialNotFound:
                            ModelState.AddModelError("Username", "That username doesn't exist");
                            break;
                        case ValidateResultError.PasswordNotValid:
                            ModelState.AddModelError("Password", "Your password is incorrect");
                            break;
                        case ValidateResultError.UserLockedOut:
                            ModelState.AddModelError("Username", "Your Account has been locked out");
                            break;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userManager.SignOutAsync(HttpContext);
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            Guid userId = Guid.Parse(base.User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _context.Users.Include(u => u.Credentials).FirstOrDefault(u => u.Id == userId);

            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Username = user.Credentials.FirstOrDefault().Username };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ChangeSecretResult changeSecretResult = _userManager.ChangePassword(model.Username, model.Password, model.OldPassword);

                if (changeSecretResult.Success)
                {
                    await _userManager.SignOutAsync(HttpContext);
                    return Json(new { model.Id });
                }
                else
                {
                    switch (changeSecretResult.Error)
                    {
                        case ChangeSecretResultError.CredentialNotFound:
                            ModelState.AddModelError("Identifier", "That username doesn't exist");
                            break;
                        case ChangeSecretResultError.OldPasswordNotValid:
                            ModelState.AddModelError("OldPassword", "Your current password is incorrect");
                            break;
                        case ChangeSecretResultError.PasswordTooWeak:
                            ModelState.AddModelError("Password", "Your new password is too weak");
                            break;
                    }
                }
            }

            //set error status
            Response.StatusCode = 400;

            //Trả về tất cả các lỗi validate model để thể hiện trên view
            return Json(ModelState.AllModels());
        }

        [HttpGet]
        public IActionResult Authority(string type = "", string id = "")
        {
            TempData["Roles"] = _context.Roles
                .OrderBy(role => role.Name).ToList();
            TempData["Users"] = _context.Users
                .OrderBy(user => user.Name).ToList();
            TempData["UserRoles"] = _context.UserRoles
                .OrderBy(user => user.Role).ToList();

            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(id))
                return View(new AuthorityModel { Type = "role", Permissions = new List<PermissionModel>() });

            AuthorityModel model = new AuthorityModel
            {
                Type = type,
                Id = Guid.Parse(id)
            };

            switch (type)
            {
                case "role":
                    {
                        _ = Guid.TryParse(id, out Guid roleId);
                        var rolePermissions = _context.RolePermissions.Where(rp => rp.RoleId == roleId).Select(rp => rp.PermissionId).ToList();
                        model.Permissions = _context.Permissions
                            .OrderBy(p => p.SequenceNo)
                            .Select(p => new PermissionModel
                            {
                                Id = p.Id,
                                SequenceNo = p.SequenceNo.ToString(),
                                Name = p.Name,
                                Description = p.Description,
                                Checked = rolePermissions.Contains(p.Id)
                            }).ToList();
                        model.Name = _context.Roles.Find(roleId).Name;
                        model.Description = _context.Roles.Find(roleId).Description;
                    }
                    break;
                case "user":
                    {
                        _ = Guid.TryParse(id, out Guid userId);
                        var userPermissions = _context.UserPermissions.Where(up => up.UserId == userId).Select(rp => rp.PermissionId).ToList();
                        model.Permissions = _context.Permissions
                            .OrderBy(p => p.SequenceNo)
                            .Select(p => new PermissionModel
                            {
                                Id = p.Id,
                                SequenceNo = p.SequenceNo.ToString(),
                                Name = p.Name,
                                Description = p.Description,
                                Checked = userPermissions.Contains(p.Id)
                            }).ToList();
                        model.Name = _context.Users.Find(userId).Name;
                        model.Description = _context.Users.Find(userId).Note;
                    }
                    break;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Authority(AuthorityModel model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (model.Permissions == null)
                    model.Permissions = new List<PermissionModel>();

                switch (model.Type)
                {
                    case "role":
                        {
                            _context.RolePermissions.RemoveRange(_context.RolePermissions.Where(rp => rp.RoleId == model.Id));
                            _context.RolePermissions.AddRange(model.Permissions.Select(p => new RolePermission { RoleId = model.Id, PermissionId = p.Id }));

                        }
                        break;
                    case "user":
                        {
                            _context.UserPermissions.RemoveRange(_context.UserPermissions.Where(rp => rp.UserId == model.Id));
                            _context.UserPermissions.AddRange(model.Permissions.Select(p => new UserPermission { UserId = model.Id, PermissionId = p.Id }));
                        }
                        break;
                }
                _context.SaveChanges();
                transaction.CommitAsync();
                return Json($"{model.Type} permissions successfully saved");
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 400;
                return Json(ex.Message);
            }
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AccessDenied()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Users
        [HttpGet]
        public IActionResult Users()
        {
            var model = new CatalogViewModel
            {
                DataUrl = Url.Action("GetUsers"),
                Columns = new List<Helpers.ColumnBootstrapTable>
                {
                    new Helpers.ColumnBootstrapTable{ field = "seqNo", title = "#", width = "30", sortable = false, formatter = "seqNoFormatter" },
                    new Helpers.ColumnBootstrapTable{ field = "name", title = "Full name", formatter = "userDrillDownFormatter" },
                    new Helpers.ColumnBootstrapTable{ field = "username", title = "Username" },
                    new Helpers.ColumnBootstrapTable{ field = "dateOfBirth", title = "DoB" },
                    new Helpers.ColumnBootstrapTable{ field = "phone", title = "Phone" },
                    new Helpers.ColumnBootstrapTable{ field = "email", title = "Email" },
                    new Helpers.ColumnBootstrapTable{ field = "address", title = "Address" },
                    new Helpers.ColumnBootstrapTable{ field = "note", title = "Note" },
                    new Helpers.ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
                }
            };

            return View("/Views/Identity/User/Users.cshtml", model);
        }
        public async Task<IActionResult> GetUsers()
        {
            List<UserViewModel> result = new List<UserViewModel>();

            var entities = _context.Users.AsQueryable();

            #region Map to ViewModel
            await entities.ForEachAsync(entity =>
             {
                 result.Add(new UserViewModel
                 {
                     Id = entity.Id.ToLowerString(),
                     Name = entity.Name,
                     Username = entity.Username,
                     Initial = entity.Initial,
                     Address = entity.Address,
                     DateOfBirth = entity.DateOfBirth.ToString("dd/MM/yyyy"),
                     Email = entity.Email,
                     Phone = entity.Phone,
                     Note = entity.Note,
                     Status = entity.Status.ToString(),
                 });
             });
            #endregion

            return Json(new { rows = result });
        }
        public IActionResult NewUser()
        {
            User model = new User()
            {
                Id = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
            };

            ViewData["Roles"] = new List<SelectListItem>(_context.Roles
                .OrderBy(role => role.SequenceNo).Select(role => new SelectListItem { Value = role.Id.ToLowerString(), Text = role.Name }));

            ViewData["Branches"] = new List<SelectListItem>(_context.Branches
                .OrderBy(branch => branch.Name).Select(branch => new SelectListItem { Value = branch.Id.ToLowerString(), Text = branch.Name }));

            return View("/Views/Identity/User/New.cshtml", model);
        }
        public IActionResult EditUser(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Users.Include(u => u.Credentials).FirstOrDefault(u => u.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.Username = model.Credentials.FirstOrDefault()?.Username;

            model.SelectedRoles = _context.UserRoles.AsNoTracking().Where(user => user.UserId == model.Id).Select(user => user.RoleId).AsEnumerable();
            model.SelectedBranches = _context.UserBranches.AsNoTracking().Where(branch => branch.UserId == model.Id).Select(branch => branch.BranchId).AsEnumerable();

            ViewData["Roles"] = new List<SelectListItem>(_context.Roles
                .OrderBy(role => role.SequenceNo).Select(role => new SelectListItem { Value = role.Id.ToLowerString(), Text = role.Name }));

            ViewData["Branches"] = new List<SelectListItem>(_context.Branches
                .OrderBy(branch => branch.Name).Select(branch => new SelectListItem { Value = branch.Id.ToLowerString(), Text = branch.Name }));

            return View("/Views/Identity/User/Edit.cshtml", model);
        }
        public IActionResult DetailUser(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Users.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).Include(u => u.Credentials).FirstOrDefault(u => u.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.Username = model.Credentials.FirstOrDefault()?.Username;

            model.SelectedRoles = _context.UserRoles.AsNoTracking().Where(user => user.UserId == model.Id).Select(user => user.RoleId).AsEnumerable();
            model.SelectedBranches = _context.UserBranches.AsNoTracking().Where(branch => branch.UserId == model.Id).Select(branch => branch.BranchId).AsEnumerable();

            ViewData["Roles"] = new List<SelectListItem>(_context.Roles
                .OrderBy(role => role.SequenceNo).Select(role => new SelectListItem { Value = role.Id.ToLowerString(), Text = role.Name }));

            ViewData["Branches"] = new List<SelectListItem>(_context.Branches
                .OrderBy(branch => branch.Name).Select(branch => new SelectListItem { Value = branch.Id.ToLowerString(), Text = branch.Name }));

            return View("/Views/Identity/User/Detail.cshtml", model);
        }
        [HttpPost]
        public IActionResult SaveUser(int mode, User model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    if (mode == 1)
                    {
                        bool duplicateUsername = _context.Users.Where(user => user.Username == model.Username).Any();
                        if (duplicateUsername)
                        {
                            ModelState.AddModelError("Username", "Username must be unique");

                            Response.StatusCode = 400;
                            return Json(ModelState.AllModels());
                        }
                        bool duplicateEmail = _context.Users.Where(user => user.Email == model.Email).Any();
                        if (duplicateEmail)
                        {
                            ModelState.AddModelError("Email", "Email must be unique");

                            Response.StatusCode = 400;
                            return Json(ModelState.AllModels());
                        }

                        _context.Users.Add(model);
                        _userManager.SignUp(model, model.Username, model.Username);
                    }
                    else
                    {
                        bool duplicateEmail = _context.Users.Where(user => user.Email == model.Email && user.Id != model.Id).Any();
                        if (duplicateEmail)
                        {
                            ModelState.AddModelError("Email", "Email must be unique");

                            Response.StatusCode = 400;
                            return Json(ModelState.AllModels());
                        }

                        var existsEntity = _context.Users.AsNoTracking().First(d => d.Id == model.Id);

                        if (existsEntity == null)
                            throw new Exception($"{model.GetType().Name} not found");

                        _context.Users.Update(model);
                    }

                    if (model.SelectedRoles == null)
                        model.SelectedRoles = new List<Guid>();

                    if (model.SelectedBranches == null)
                        model.SelectedBranches = new List<Guid>();

                    _context.UserRoles.RemoveRange(_context.UserRoles.Where(userRole => userRole.UserId == model.Id));
                    _context.UserRoles.AddRange(model.SelectedRoles.Select(roleId => new UserRole { UserId = model.Id, RoleId = roleId }));
                    _context.UserBranches.RemoveRange(_context.UserBranches.Where(branchUser => branchUser.UserId == model.Id));
                    _context.UserBranches.AddRange(model.SelectedBranches.Select(branchId => new UserBranch { UserId = model.Id, BranchId = branchId }));
                    _context.SaveChanges();
                    transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    transaction.RollbackAsync();
                    Response.StatusCode = 500;
                    return Json(ex.InnerException.Message);
                }

                return Json(new { model.Id, model.Name });
            }

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }
        [HttpPost]
        public IActionResult DeleteUser(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Users.Find(modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }


            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Credentials.RemoveRange(_context.Credentials.Where(u => u.UserId == model.Id));
                _context.UserPermissions.RemoveRange(_context.UserPermissions.Where(u => u.UserId == model.Id));
                _context.UserRoles.RemoveRange(_context.UserRoles.Where(u => u.UserId == model.Id));
                _context.Users.Remove(model);
                _context.SaveChanges();
                transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json(ex.InnerException.Message);
            }
            return Ok();
        }
        #endregion

        #region Roles
        [HttpGet]
        public IActionResult Roles()
        {
            var model = new CatalogViewModel
            {
                DataUrl = Url.Action("GetRoles"),
                Columns = new List<Helpers.ColumnBootstrapTable>
                {
                    new Helpers.ColumnBootstrapTable{ field = "seqNo", title = "#", width = "30", sortable = false, formatter = "seqNoFormatter" },
                    new Helpers.ColumnBootstrapTable{ field = "sequenceNo", title = "Level", width = "30" },
                    new Helpers.ColumnBootstrapTable{ field = "name", title = "Name", formatter = "roleDrillDownFormatter" },
                    new Helpers.ColumnBootstrapTable{ field = "description", title = "Description" },
                    new Helpers.ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
                }
            };

            return View("/Views/Identity/Role/Roles.cshtml", model);
        }
        public async Task<IActionResult> GetRoles()
        {
            List<RoleViewModel> result = new List<RoleViewModel>();

            var entities = _context.Roles.AsQueryable();

            #region Map to ViewModel
            await entities.ForEachAsync(entity =>
             {
                 result.Add(new RoleViewModel
                 {
                     Id = entity.Id.ToLowerString(),
                     SequenceNo = entity.SequenceNo.ToString(),
                     Name = entity.Name,
                     Description = entity.Description,
                     Status = entity.Status.ToString(),
                 });
             });
            #endregion

            return Json(new { rows = result });
        }
        public IActionResult NewRole()
        {
            var sqn = _context.Roles.Max(r => r.SequenceNo);
            Role model = new Role()
            {
                Id = Guid.NewGuid(),
                SequenceNo = ++sqn,
            };
            return View("/Views/Identity/Role/New.cshtml", model);
        }
        public IActionResult EditRole(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Roles.FirstOrDefault(u => u.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Identity/Role/Edit.cshtml", model);
        }
        public IActionResult DetailRole(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Roles.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(u => u.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Identity/Role/Detail.cshtml", model);
        }
        [HttpPost]
        public IActionResult SaveRole(int mode, Role model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    if (mode == 1)
                    {
                        _context.Roles.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Roles.AsNoTracking().First(d => d.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.Roles.Update(model);
                    }
                    _context.SaveChanges();
                    transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    transaction.RollbackAsync();
                    Response.StatusCode = 500;
                    return Json(ex.InnerException.Message);
                }

                return Json(new { model.Id, model.Name });
            }

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }
        [HttpPost]
        public IActionResult DeleteRole(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Roles.Find(modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }


            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.RolePermissions.RemoveRange(_context.RolePermissions.Where(u => u.RoleId == model.Id));
                _context.UserRoles.RemoveRange(_context.UserRoles.Where(u => u.RoleId == model.Id));
                _context.Roles.Remove(model);
                _context.SaveChanges();
                transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json(ex.InnerException.Message);
            }
            return Ok();
        }
        #endregion

        #region Permissions
        [HttpGet]
        public IActionResult Permissions()
        {
            var model = new CatalogViewModel
            {
                DataUrl = Url.Action("GetPermissions"),
                Columns = new List<Helpers.ColumnBootstrapTable>
                {
                    new Helpers.ColumnBootstrapTable{ field = "seqNo", title = "#", width = "30", sortable = false, formatter = "seqNoFormatter" },
                    new Helpers.ColumnBootstrapTable{ field = "sequenceNo", title = "Level", width = "30" },
                    new Helpers.ColumnBootstrapTable{ field = "name", title = "Name", formatter = "permissionDrillDownFormatter" },
                    new Helpers.ColumnBootstrapTable{ field = "description", title = "Description" },
                    new Helpers.ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
                }
            };

            return View("/Views/Identity/Permission/Permissions.cshtml", model);
        }
        public async Task<IActionResult> GetPermissions()
        {
            List<PermissionViewModel> result = new List<PermissionViewModel>();

            var entities = _context.Permissions.AsQueryable();

            #region Map to ViewModel
            await entities.ForEachAsync(entity =>
             {
                 result.Add(new PermissionViewModel
                 {
                     Id = entity.Id.ToLowerString(),
                     SequenceNo = entity.SequenceNo.ToString(),
                     Name = entity.Name,
                     Description = entity.Description,
                     Status = entity.Status.ToString(),
                 });
             });
            #endregion

            return Json(new { rows = result });
        }
        public IActionResult NewPermission()
        {
            var sqn = _context.Permissions.Max(r => r.SequenceNo);
            Permission model = new Permission()
            {
                Id = Guid.NewGuid(),
                SequenceNo = ++sqn,
            };
            return View("/Views/Identity/Permission/New.cshtml", model);
        }
        public IActionResult EditPermission(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Permissions.FirstOrDefault(u => u.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }
            return View("/Views/Identity/Permission/Edit.cshtml", model);
        }
        public IActionResult DetailPermission(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Permissions.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(u => u.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Identity/Permission/Detail.cshtml", model);
        }
        [HttpPost]
        public IActionResult SavePermission(int mode, Permission model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    if (mode == 1)
                    {
                        _context.Permissions.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Permissions.AsNoTracking().First(d => d.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }
                        _context.Permissions.Update(model);
                    }
                    _context.SaveChanges();
                    transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    transaction.RollbackAsync();
                    Response.StatusCode = 500;
                    return Json(ex.InnerException.Message);
                }
                return Json(new { model.Id, model.Name });
            }

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }
        [HttpPost]
        public IActionResult DeletePermission(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Permissions.Find(modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }


            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.RolePermissions.RemoveRange(_context.RolePermissions.Where(u => u.PermissionId == model.Id));
                _context.UserPermissions.RemoveRange(_context.UserPermissions.Where(u => u.PermissionId == model.Id));
                _context.Permissions.Remove(model);
                _context.SaveChanges();
                transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json(ex.InnerException.Message);
            }
            return Ok();
        }
        #endregion

        #region Currencies
        [HttpGet]
        public IActionResult Currencies()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetCurrencies"),
                Columns = new List<Helpers.ColumnBootstrapTable>
                {
                    new Helpers.ColumnBootstrapTable{ field = "seqNo", title = "#", width = "30", sortable = false, formatter = "seqNoFormatter" },
                    new Helpers.ColumnBootstrapTable{ field = "name", title = "Full name", formatter = "currencyDrillDownFormatter" },
                    new Helpers.ColumnBootstrapTable{ field = "exchangeRate", title = "ExchangeRate" },
                    new Helpers.ColumnBootstrapTable{ field = "note", title = "Note" },
                    new Helpers.ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
                }
            };

            return View("/Views/Identity/Currency/Currencies.cshtml", model);
        }
        public async Task<IActionResult> GetCurrencies()
        {
            List<CurrencyViewModel> result = new List<CurrencyViewModel>();

            var entities = _context.Currencies.AsQueryable();

            #region Map to ViewModel
            await entities.ForEachAsync(entity =>
            {
                result.Add(new CurrencyViewModel
                {
                    Id = entity.Id.ToLowerString(),
                    Name = entity.Name,
                    ExchangeRate = entity.ExchangeRate,
                    Note = entity.Note,
                    Status = entity.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }
        public IActionResult NewCurrency()
        {
            Currency model = new Currency
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Identity/Currency/New.cshtml", model);
        }
        public IActionResult EditCurrency(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Currencies.FirstOrDefault(u => u.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }
            return View("/Views/Identity/Currency/Edit.cshtml", model);
        }
        public IActionResult DetailCurrency(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Currencies.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(u => u.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Identity/Currency/Detail.cshtml", model);
        }
        [HttpPost]
        public IActionResult SaveCurrency(int mode, Currency model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    if (mode == 1)
                    {
                        _context.Currencies.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Currencies.AsNoTracking().First(d => d.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }
                        _context.Currencies.Update(model);
                    }
                    _context.SaveChanges();
                    transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    transaction.RollbackAsync();
                    Response.StatusCode = 500;
                    return Json(ex.InnerException.Message);
                }
                return Json(new { model.Id, model.Name });
            }

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }
        [HttpPost]
        public IActionResult DeleteCurrency(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Currencies.Find(modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }


            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Currencies.Remove(model);
                _context.SaveChanges();
                transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json(ex.InnerException.Message);
            }
            return Ok();
        }
        #endregion
        public IActionResult Profile(string id)
        {
            _ = Guid.TryParse(id, out Guid userId);
            var model = _context.Users.Include(u => u.Credentials).FirstOrDefault(u => u.Id == userId);

            if (model == null)
            {
                return View("_404");
            }

            model.Username = model.Credentials.FirstOrDefault()?.Username;

            model.SelectedRoles = _context.UserRoles.AsNoTracking().Where(user => user.UserId == model.Id).Select(user => user.RoleId).AsEnumerable();

            ViewData["Roles"] = new List<SelectListItem>(_context.Roles
                .OrderBy(role => role.SequenceNo).Select(role => new SelectListItem { Value = role.Id.ToLowerString(), Text = role.Name }));

            return View(model);
        }

        [HttpPost]
        public IActionResult Profile(User model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var existsEntity = _context.Users.Find(model.Id);

                    if (model == null)
                    {
                        Response.StatusCode = 404;
                        return Json($"{model.GetType().Name} not found");
                    }

                    existsEntity.Name = model.Name;
                    existsEntity.Initial = model.Initial;
                    existsEntity.DateOfBirth = model.DateOfBirth;
                    existsEntity.Phone = model.Phone;
                    existsEntity.Email = model.Email;
                    existsEntity.Address = model.Address;
                    existsEntity.Note = model.Note;

                    _context.Users.Update(existsEntity);
                    _context.SaveChanges();
                    transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    transaction.RollbackAsync();
                    Response.StatusCode = 500;
                    return Json(ex.InnerException.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult ResetPassword(string username)
        {
            ChangeSecretResult changeSecretResult = _userManager.ResetPassword(username);
            if (!changeSecretResult.Success)
            {                //set error status
                Response.StatusCode = 400;
            }
            return Json(new { changeSecretResult });

        }
    }
}
