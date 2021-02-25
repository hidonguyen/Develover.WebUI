using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Develover.WebUI.DbContexts;
using Develover.WebUI.Entities;
using Develover.WebUI.Extensions;
using Develover.WebUI.Identity;
using Develover.WebUI.Models;
using System.ComponentModel.DataAnnotations;
using Develover.WebUI.Helpers;
using System.Collections;
using System.Threading.Tasks;

namespace Develover.WebUI.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly DataContext _context;

        public CatalogController(ILogger<CatalogController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region Departments

        [HttpGet]
        public IActionResult Departments()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetDepartments")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "departmentDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/Department/Departments.cshtml", model);
        }

        public async Task<IActionResult> GetDepartments()
        {
            List<DepartmentViewModel> result = new List<DepartmentViewModel>();

            var departments = _context.Departments
                .AsQueryable();

            #region Map entity to ViewModel
            await departments.ForEachAsync(department =>
            {
                result.Add(new DepartmentViewModel
                {
                    Id = department.Id.ToLowerString(),
                    Name = department.Name,
                    Note = department.Note,
                    Status = department.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewDepartment()
        {
            var model = new Department
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/Department/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditDepartment(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Departments.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Department/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailDepartment(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Departments.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Department/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveDepartment(int mode, Department model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.Departments.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Departments.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.Departments.Update(model);
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
        public IActionResult DeleteDepartment(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Departments.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Departments.Remove(model);

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

        #region Document Categories

        [HttpGet]
        public IActionResult DocumentCategories()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetDocumentCategories")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "documentCategoryDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/DocumentCategory/DocumentCategories.cshtml", model);
        }

        public async Task<IActionResult> GetDocumentCategories()
        {
            List<DocumentCategoryViewModel> result = new List<DocumentCategoryViewModel>();

            var documentCategories = _context.DocumentCategories
                .AsQueryable();

            #region Map entity to ViewModel
            await documentCategories.ForEachAsync(documentCategory =>
            {
                result.Add(new DocumentCategoryViewModel
                {
                    Id = documentCategory.Id.ToLowerString(),
                    Name = documentCategory.Name,
                    Note = documentCategory.Note,
                    Status = documentCategory.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewDocumentCategory()
        {
            var model = new DocumentCategory
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/DocumentCategory/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditDocumentCategory(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.DocumentCategories.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/DocumentCategory/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailDocumentCategory(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.DocumentCategories.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/DocumentCategory/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveDocumentCategory(int mode, DocumentCategory model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.DocumentCategories.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.DocumentCategories.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.DocumentCategories.Update(model);
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
        public IActionResult DeleteDocumentCategory(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.DocumentCategories.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.DocumentCategories.Remove(model);

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

        #region Document Status

        [HttpGet]
        public IActionResult DocumentStatus()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetDocumentStatus")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "documentStatusDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/DocumentStatus/DocumentStatus.cshtml", model);
        }

        public async Task<IActionResult> GetDocumentStatus()
        {
            List<DocumentStatusViewModel> result = new List<DocumentStatusViewModel>();

            var documentStatus = _context.DocumentStatus
                .AsQueryable();

            #region Map entity to ViewModel
            await documentStatus.ForEachAsync(status =>
            {
                result.Add(new DocumentStatusViewModel
                {
                    Id = status.Id.ToLowerString(),
                    Name = status.Name,
                    Note = status.Note,
                    Status = status.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewDocumentStatus()
        {
            var model = new DocumentStatus
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/DocumentStatus/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditDocumentStatus(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.DocumentStatus.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/DocumentStatus/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailDocumentStatus(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.DocumentStatus.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/DocumentStatus/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveDocumentStatus(int mode, DocumentStatus model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.DocumentStatus.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.DocumentStatus.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.DocumentStatus.Update(model);
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
        public IActionResult DeleteDocumentStatus(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.DocumentStatus.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.DocumentStatus.Remove(model);

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

        #region Document Types

        [HttpGet]
        public IActionResult DocumentTypes()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetDocumentTypes")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "documentTypeDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/DocumentType/DocumentTypes.cshtml", model);
        }

        public async Task<IActionResult> GetDocumentTypes()
        {
            List<DocumentTypeViewModel> result = new List<DocumentTypeViewModel>();

            var documentTypes = _context.DocumentTypes
                .AsQueryable();

            #region Map entity to ViewModel
            await documentTypes.ForEachAsync(documentType =>
            {
                result.Add(new DocumentTypeViewModel
                {
                    Id = documentType.Id.ToLowerString(),
                    Name = documentType.Name,
                    Note = documentType.Note,
                    Status = documentType.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewDocumentType()
        {
            var model = new DocumentType
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/DocumentType/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditDocumentType(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.DocumentTypes.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/DocumentType/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailDocumentType(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.DocumentTypes.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/DocumentType/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveDocumentType(int mode, DocumentType model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.DocumentTypes.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.DocumentTypes.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.DocumentTypes.Update(model);
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
        public IActionResult DeleteDocumentType(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.DocumentTypes.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.DocumentTypes.Remove(model);

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

        #region Employees
        private static List<ColumnBootstrapTable> DetailDependentColumns => new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "operate", width = "70", formatter = "operateFormatter", events = "window.operateEvents", clickToSelect = false, sortable = false },
                    new ColumnBootstrapTable { field = "sequenceNo", title = "#", sortable = false, width = "20" },
                    new ColumnBootstrapTable { field = "firstName", visible = false },
                    new ColumnBootstrapTable { field = "middleName", visible = false  },
                    new ColumnBootstrapTable { field = "lastName", visible = false  },
                    new ColumnBootstrapTable { field = "fullName", title = "Full Name" },
                    new ColumnBootstrapTable { field = "relationship", title = "Relationship" },
                    //EmployeeBiography
                    new ColumnBootstrapTable { field = "dateOfBirth", title = "Date Of Birth", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "placeOfBirth", visible = false },
                    new ColumnBootstrapTable { field = "genderId", visible = false },
                    new ColumnBootstrapTable { field = "gender", title = "Gender" },
                    new ColumnBootstrapTable { field = "maritalStatusId", visible = false },
                    new ColumnBootstrapTable { field = "maritalStatus", title = "Marital Status" },
                    new ColumnBootstrapTable { field = "ethnicGroupId", visible = false },
                    new ColumnBootstrapTable { field = "ethnicGroup", visible = false },
                    //PermanentAddress
                    new ColumnBootstrapTable { field = "permanentAddressId", visible = false },
                    new ColumnBootstrapTable { field = "permanentAddressLine1", visible = false },
                    new ColumnBootstrapTable { field = "permanentAddressLine2", visible = false },
                    new ColumnBootstrapTable { field = "permanentAddressCity", visible = false },
                    new ColumnBootstrapTable { field = "permanentAddressCountry", visible = false },
                    new ColumnBootstrapTable { field = "permanentAddressNote", visible = false },
                    new ColumnBootstrapTable { field = "permanentAddress", title = "Permanent Address"},
                    //TemporaryAddress
                    new ColumnBootstrapTable { field = "temporaryAddressId", visible = false },
                    new ColumnBootstrapTable { field = "temporaryAddressLine1", visible = false },
                    new ColumnBootstrapTable { field = "temporaryAddressLine2", visible = false },
                    new ColumnBootstrapTable { field = "temporaryAddressCity", visible = false },
                    new ColumnBootstrapTable { field = "temporaryAddressCountry", visible = false },
                    new ColumnBootstrapTable { field = "temporaryAddressNote", visible = false },
                    new ColumnBootstrapTable { field = "temporaryAddress", title = "Temporary Address" },
                    //                
                    new ColumnBootstrapTable { field = "email", title = "Email"},
                    new ColumnBootstrapTable { field = "phone", title = "Phone"},
                    new ColumnBootstrapTable { field = "alternativePhone", visible = false },

                    new ColumnBootstrapTable { field = "note", title = "Note" }
        };

        private void SetEmployeePickerSource()
        {
            ViewData["Genders"] = _context.Genders.AsNoTracking().OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["MaritalStatus"] = _context.MaritalStatus.AsNoTracking().OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["EthnicGroups"] = _context.EthnicGroups.AsNoTracking().OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Divisions"] = _context.Divisions.AsNoTracking().OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Positions"] = _context.Positions.AsNoTracking().OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Departments"] = _context.Departments.AsNoTracking().OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
        }

        [HttpPost]
        public IActionResult ValidateEmployeeDependentModel(EmployeeDependent model)
        {
            if (ModelState.IsValid)
                return Ok(new EmployeeDependentViewModel
                {
                    Id = model.Id,
                    SequenceNo = model.SequenceNo,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    FullName = model.FullName,
                    Relationship = model.Relationship,
                    //itemBiography
                    DateOfBirth = model.DateOfBirth,
                    PlaceOfBirth = model.PlaceOfBirth,
                    GenderId = model.GenderId,
                    Gender = model.Gender?.Name,
                    MaritalStatusId = model.MaritalStatusId,
                    MaritalStatus = model.MaritalStatus?.Name,
                    EthnicGroupId = model.EthnicGroupId,
                    EthnicGroup = model.EthnicGroup?.Name,

                    //PermanentAddress
                    PermanentAddressId = model.PermanentAddressId,
                    PermanentAddressLine1 = model.PermanentAddress?.Line1,
                    PermanentAddressLine2 = model.PermanentAddress?.Line2,
                    PermanentAddressCity = model.PermanentAddress?.City,
                    PermanentAddressCountry = model.PermanentAddress?.Country,
                    PermanentAddressNote = model.PermanentAddress?.Note,
                    PermanentAddress = model.PermanentAddress?.ToString(),

                    //TemporaryAddress
                    TemporaryAddressId = model.TemporaryAddressId,
                    TemporaryAddressLine1 = model.TemporaryAddress?.Line1,
                    TemporaryAddressLine2 = model.TemporaryAddress?.Line2,
                    TemporaryAddressCity = model.TemporaryAddress?.City,
                    TemporaryAddressCountry = model.TemporaryAddress?.Country,
                    TemporaryAddressNote = model.TemporaryAddress?.Note,
                    TemporaryAddress = model.TemporaryAddress?.ToString(),
                    //                
                    Email = model.Email,
                    Phone = model.Phone,
                    AlternativePhone = model.AlternativePhone,

                    Note = model.Note,
                });

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }

        [HttpGet]
        public IActionResult Employees()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetEmployees")
            };

            model.Columns = new List<ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "avatar", title = "Avatar", formatter = "imageFormatter" },
                new ColumnBootstrapTable{ field = "fullName", title = "Full Name", formatter = "employeeDrillDownFormatter" },
                //EmployeeIdentification
                new ColumnBootstrapTable{ field = "identificationNo", title = "Identification No"},
                //                
                new ColumnBootstrapTable{ field = "taxIdentificationNo", title = "Tax Identification No"},
                //EmployeeBiography
                new ColumnBootstrapTable{ field = "biographyDateOfBirth", title = "Date Of Birth", formatter = "dateFormatter", sorter = "dateSorter"},
                new ColumnBootstrapTable{ field = "biographyPlaceOfBirth", title = "Place Of Birth"},
                new ColumnBootstrapTable{ field = "biographyGender", title = "Gender"},
                new ColumnBootstrapTable{ field = "biographyMaritalStatus", title = "Marital Status"},
                new ColumnBootstrapTable{ field = "biographyEthnicGroup", title = "Ethnic"},
                //                
                new ColumnBootstrapTable{ field = "email", title = "Email"},
                new ColumnBootstrapTable{ field = "phone", title = "Phone"},
                //PermanentAddress
                new ColumnBootstrapTable{ field = "permanentAddress", title = "Permanent Address"},
                //TemporaryAddress
                new ColumnBootstrapTable{ field = "temporaryAddress", title = "Temporary Address"},
                //
                new ColumnBootstrapTable{ field = "hireDate", title = "HireDate", formatter = "dateFormatter", sorter = "dateSorter"},
                //
                new ColumnBootstrapTable{ field = "division", title = "Division"},
                new ColumnBootstrapTable{ field = "position", title = "Position"},
                //
                new ColumnBootstrapTable{ field = "socialInsuranceNo", title = "Social Insurance No"},
                new ColumnBootstrapTable{ field = "socialInsuranceDate", title = "Social Insurance Date", formatter = "dateFormatter", sorter = "dateSorter"},
                new ColumnBootstrapTable{ field = "healthInsuranceNo", title = "Health Insurance No"},
                new ColumnBootstrapTable{ field = "healthInsuranceDate", title = "Health Insurance Date", formatter = "dateFormatter", sorter = "dateSorter"},

                new ColumnBootstrapTable{ field = "alternativePhone", title = "Alternative Phone"},
                new ColumnBootstrapTable{ field = "note", title = "Note" },

                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/Employee/Employees.cshtml", model);
        }

        public async Task<IActionResult> GetEmployees()
        {
            List<EmployeeViewModel> result = new List<EmployeeViewModel>();

            var employees = _context.Employees.Include(e => e.Identification)
                .Include(e => e.Biography).ThenInclude(i => i.Gender)
                .Include(e => e.Biography).ThenInclude(i => i.MaritalStatus)
                .Include(e => e.Biography).ThenInclude(i => i.EthnicGroup)
                .Include(e => e.Division)
                .Include(e => e.Position)
                .Include(e => e.PermanentAddress)
                .Include(e => e.TemporaryAddress)
                .AsQueryable();

            #region Map entity to ViewModel
            await employees.ForEachAsync(employee =>
            {
                result.Add(new EmployeeViewModel
                {
                    Id = employee.Id,
                    FullName = employee.FullName,
                    //EmployeeIdentification
                    IdentificationNo = employee.Identification?.No,
                    PersonalTaxCode = employee.PersonalTaxCode,
                    //EmployeeBiography
                    DateOfBirth = employee.Biography?.DateOfBirth,
                    PlaceOfBirth = employee.Biography?.PlaceOfBirth,
                    Gender = employee.Biography.Gender?.Name,
                    MaritalStatus = employee.Biography?.MaritalStatus?.Name,
                    EthnicGroup = employee.Biography?.EthnicGroup?.Name,
                    //PermanentAddress
                    PermanentAddress = employee.PermanentAddress?.ToString(),
                    TemporaryAddress = employee.TemporaryAddress?.ToString(),
                    //                
                    Email = employee.Email,
                    Phone = employee.Phone,
                    //
                    HireDate = employee.HireDate,
                    //
                    Division = employee.Division?.Name,
                    Position = employee.Position?.Name,
                    //
                    SocialInsuranceNo = employee.SocialInsuranceNo,
                    SocialInsuranceDate = employee.SocialInsuranceDate,
                    HealthInsuranceNo = employee.HealthInsuranceNo,
                    HealthInsuranceDate = employee.HealthInsuranceDate,

                    AlternativePhone = employee.AlternativePhone,
                    Note = employee.Note,
                    Status = employee.Status,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        public async Task<IActionResult> GetEmployeeDependents(string id)
        {
            List<EmployeeDependentViewModel> result = new List<EmployeeDependentViewModel>();

            _ = Guid.TryParse(id, out Guid employeeId);

            var items = _context.EmployeeDependents
                .Include(i => i.Gender)
                .Include(i => i.MaritalStatus)
                .Include(i => i.EthnicGroup)
                .Include(e => e.PermanentAddress)
                .Include(e => e.TemporaryAddress)
                .OrderBy(i => i.SequenceNo)
                .Where(i => i.EmployeeId == employeeId)
                .AsQueryable();


            #region Map entity to ViewModel
            await items.ForEachAsync(item =>
            {
                result.Add(new EmployeeDependentViewModel
                {
                    Id = item.Id,
                    SequenceNo = item.SequenceNo,
                    FirstName = item.FirstName,
                    MiddleName = item.MiddleName,
                    LastName = item.LastName,
                    FullName = item.FullName,
                    Relationship = item.Relationship,
                    //itemBiography
                    DateOfBirth = item.DateOfBirth,
                    PlaceOfBirth = item.PlaceOfBirth,
                    GenderId = item.GenderId,
                    Gender = item.Gender?.Name,
                    MaritalStatusId = item.MaritalStatusId,
                    MaritalStatus = item.MaritalStatus?.Name,
                    EthnicGroupId = item.EthnicGroupId,
                    EthnicGroup = item.EthnicGroup?.Name,

                    //PermanentAddress
                    PermanentAddressId = item.PermanentAddressId,
                    PermanentAddressLine1 = item.PermanentAddress?.Line1,
                    PermanentAddressLine2 = item.PermanentAddress?.Line2,
                    PermanentAddressCity = item.PermanentAddress?.City,
                    PermanentAddressCountry = item.PermanentAddress?.Country,
                    PermanentAddressNote = item.PermanentAddress?.Note,
                    PermanentAddress = item.PermanentAddress?.ToString(),

                    //TemporaryAddress
                    TemporaryAddressId = item.TemporaryAddressId,
                    TemporaryAddressLine1 = item.TemporaryAddress?.Line1,
                    TemporaryAddressLine2 = item.TemporaryAddress?.Line2,
                    TemporaryAddressCity = item.TemporaryAddress?.City,
                    TemporaryAddressCountry = item.TemporaryAddress?.Country,
                    TemporaryAddressNote = item.TemporaryAddress?.Note,
                    TemporaryAddress = item.TemporaryAddress?.ToString(),
                    //                
                    Email = item.Email,
                    Phone = item.Phone,
                    AlternativePhone = item.AlternativePhone,

                    Note = item.Note,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewEmployee()
        {
            var model = new Employee
            {
                Id = Guid.NewGuid(),
                IdentificationId = Guid.NewGuid(),
                BiographyId = Guid.NewGuid(),
                PermanentAddressId = Guid.NewGuid(),
                TemporaryAddressId = Guid.NewGuid(),
                Avatar = Array.Empty<byte>()
            };

            model.DetailDependentDataUrl = $"/catalog/getemployeedependents?id={model.Id}";
            model.DetailDependentColumns = DetailDependentColumns;

            SetEmployeePickerSource();
            return View("/Views/Catalog/Employee/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditEmployee(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Employees
                .Include(e => e.Identification)
                .Include(e => e.Biography)
                .Include(e => e.PermanentAddress)
                .Include(e => e.TemporaryAddress)
                .Include(e => e.Dependents)
                .FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            if (model.Avatar == null) model.Avatar = Array.Empty<byte>();

            model.DetailDependentDataUrl = $"/catalog/getemployeedependents?id={model.Id}";
            model.DetailDependentColumns = DetailDependentColumns;

            SetEmployeePickerSource();
            return View("/Views/Catalog/Employee/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailEmployee(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Employees
                .Include(e => e.Identification)
                .Include(e => e.Biography)
                .Include(e => e.PermanentAddress)
                .Include(e => e.TemporaryAddress)
                .Include(e => e.Dependents)
                .Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            if (model.Avatar == null) model.Avatar = Array.Empty<byte>();

            model.DetailDependentDataUrl = $"/catalog/getemployeedependents?id={model.Id}";
            model.DetailDependentColumns = DetailDependentColumns;

            SetEmployeePickerSource();
            return View("/Views/Catalog/Employee/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveEmployee(int mode, Employee model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.Employees.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Employees.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.EmployeeDependents.RemoveRange(_context.EmployeeDependents.Where(e => e.EmployeeId == existsEntity.Id));

                        if (model.Dependents == null)
                            model.Dependents = new List<EmployeeDependent>();

                        _context.EmployeeDependents.AddRange(model.Dependents);

                        _context.Employees.Update(model);
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

                return Json(new { model.Id, model.FullName });
            }

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }

        [HttpPost]
        public IActionResult DeleteEmployee(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Employees
                .Include(e => e.Biography)
                .Include(e => e.Identification)
                .Include(e => e.PermanentAddress)
                .Include(e => e.TemporaryAddress)
                .Include(e => e.Dependents).ThenInclude(e => e.TemporaryAddress)
                .Include(e => e.Dependents).ThenInclude(e => e.PermanentAddress)
                .FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {

                _context.EmployeeIdentifications.Remove(model.Identification);
                _context.EmployeeBiographies.Remove(model.Biography);
                _context.EmployeeAddresses.Remove(model.TemporaryAddress);
                _context.EmployeeAddresses.Remove(model.PermanentAddress);
                _context.EmployeeAddresses.Remove(model.Dependents.First().PermanentAddress);
                _context.EmployeeAddresses.Remove(model.Dependents.First().TemporaryAddress);
                _context.EmployeeDependents.RemoveRange(model.Dependents);
                _context.Employees.Remove(model);

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

        #region Published Places

        [HttpGet]
        public IActionResult PublishedPlaces()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetPublishedPlaces")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "publishedPlaceDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/PublishedPlace/PublishedPlaces.cshtml", model);
        }

        public async Task<IActionResult> GetPublishedPlaces()
        {
            List<PublishedPlaceViewModel> result = new List<PublishedPlaceViewModel>();

            var publishedPlaces = _context.PublishedPlaces
                .AsQueryable();

            #region Map entity to ViewModel
            await publishedPlaces.ForEachAsync(publishedPlace =>
            {
                result.Add(new PublishedPlaceViewModel
                {
                    Id = publishedPlace.Id.ToLowerString(),
                    Name = publishedPlace.Name,
                    Note = publishedPlace.Note,
                    Status = publishedPlace.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewPublishedPlace()
        {
            var model = new PublishedPlace
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/PublishedPlace/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditPublishedPlace(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.PublishedPlaces.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/PublishedPlace/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailPublishedPlace(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.PublishedPlaces.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/PublishedPlace/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SavePublishedPlace(int mode, PublishedPlace model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.PublishedPlaces.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.PublishedPlaces.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.PublishedPlaces.Update(model);
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
        public IActionResult DeletePublishedPlace(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.PublishedPlaces.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.PublishedPlaces.Remove(model);

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

        #region StockItems

        private void SetSelectPickerSourceStockItems()
        {
            ViewData["Locations"] = _context.Locations.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["UnitOfMeasures"] = _context.UnitOfMeasures.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
        }
        [HttpGet]
        public IActionResult StockItems()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetStockItems")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "stockItemDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "unitOfMeasureId", visible = false },
                new ColumnBootstrapTable{ field = "unitOfMeasure", title = "UOM" },
                new ColumnBootstrapTable{ field = "defaultLocationId", visible = false },
                new ColumnBootstrapTable{ field = "defaultLocation", title = "Default Location" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            SetSelectPickerSourceStockItems();
            return View("/Views/Catalog/StockItem/StockItems.cshtml", model);
        }

        public async Task<IActionResult> GetStockItems()
        {
            List<StockItemViewModel> result = new List<StockItemViewModel>();

            var stockItems = _context.StockItems
                .Include(i => i.UnitOfMeasure)
                .Include(i => i.DefaultLocation)
                .AsQueryable();

            #region Map entity to ViewModel
            await stockItems.ForEachAsync(item =>
            {
                result.Add(new StockItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    UnitOfMeasureId = item.UnitOfMeasureId,
                    UnitOfMeasure = item.UnitOfMeasure.Name,
                    DefaultLocationId = item.DefaultLocationId,
                    DefaultLocation = item.DefaultLocation?.Name,
                    Note = item.Note,
                    Status = item.Status,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewStockItem()
        {
            var model = new StockItem
            {
                Id = Guid.NewGuid()
            };

            SetSelectPickerSourceStockItems();
            return View("/Views/Catalog/StockItem/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditStockItem(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.StockItems.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            SetSelectPickerSourceStockItems();
            return View("/Views/Catalog/StockItem/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailStockItem(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.StockItems.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }
            SetSelectPickerSourceStockItems();
            return View("/Views/Catalog/StockItem/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveStockItem(int mode, StockItem model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.StockItems.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.StockItems.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.StockItems.Update(model);
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
        public IActionResult DeleteStockItem(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.StockItems.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.StockItems.Remove(model);

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

        #region Locations

        [HttpGet]
        public IActionResult Locations()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetLocations")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "locationDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/Location/Locations.cshtml", model);
        }

        public async Task<IActionResult> GetLocations()
        {
            List<LocationViewModel> result = new List<LocationViewModel>();

            var locations = _context.Locations
                .AsQueryable();

            #region Map entity to ViewModel
            await locations.ForEachAsync(location =>
            {
                result.Add(new LocationViewModel
                {
                    Id = location.Id.ToLowerString(),
                    Name = location.Name,
                    Note = location.Note,
                    Status = location.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewLocation()
        {
            var model = new Location
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/Location/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditLocation(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Locations.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Location/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailLocation(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Locations.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Location/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveLocation(int mode, Location model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.Locations.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Locations.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.Locations.Update(model);
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
        public IActionResult DeleteLocation(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Locations.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Locations.Remove(model);

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

        #region UnitOfMeasures

        [HttpGet]
        public IActionResult UnitOfMeasures()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetUnitOfMeasures")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "unitOfMeasureDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/UnitOfMeasure/UnitOfMeasures.cshtml", model);
        }

        public async Task<IActionResult> GetUnitOfMeasures()
        {
            List<UnitOfMeasureViewModel> result = new List<UnitOfMeasureViewModel>();

            var unitOfMeasures = _context.UnitOfMeasures
                .AsQueryable();

            #region Map entity to ViewModel
            await unitOfMeasures.ForEachAsync(unitOfMeasure =>
            {
                result.Add(new UnitOfMeasureViewModel
                {
                    Id = unitOfMeasure.Id.ToLowerString(),
                    Name = unitOfMeasure.Name,
                    Note = unitOfMeasure.Note,
                    Status = unitOfMeasure.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewUnitOfMeasure()
        {
            var model = new UnitOfMeasure
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/UnitOfMeasure/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditUnitOfMeasure(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.UnitOfMeasures.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/UnitOfMeasure/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailUnitOfMeasure(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.UnitOfMeasures.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/UnitOfMeasure/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveUnitOfMeasure(int mode, UnitOfMeasure model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.UnitOfMeasures.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.UnitOfMeasures.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.UnitOfMeasures.Update(model);
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
        public IActionResult DeleteUnitOfMeasure(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.UnitOfMeasures.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.UnitOfMeasures.Remove(model);

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

        #region Suppliers

        [HttpGet]
        public IActionResult Suppliers()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetSuppliers")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "supplierDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "taxCode", title = "Tax Code" },
                new ColumnBootstrapTable{ field = "phone", title = "Phone" },
                new ColumnBootstrapTable{ field = "email", title = "Email" },
                new ColumnBootstrapTable{ field = "address", title = "Address" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/Supplier/Suppliers.cshtml", model);
        }

        public async Task<IActionResult> GetSuppliers()
        {
            List<SupplierViewModel> result = new List<SupplierViewModel>();

            var suppliers = _context.Suppliers
                .AsQueryable();

            #region Map entity to ViewModel
            await suppliers.ForEachAsync(supplier =>
            {
                result.Add(new SupplierViewModel
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    TaxCode = supplier.TaxCode,
                    Phone = supplier.Phone,
                    Email = supplier.Email,
                    Address = supplier.Address,
                    Note = supplier.Note,
                    Status = supplier.Status,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewSupplier()
        {
            var model = new Supplier
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/Supplier/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditSupplier(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Suppliers.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Supplier/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailSupplier(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Suppliers.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Supplier/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveSupplier(int mode, Supplier model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.Suppliers.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Suppliers.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.Suppliers.Update(model);
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
        public IActionResult DeleteSupplier(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Suppliers.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Suppliers.Remove(model);

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

        #region Vehicles

        private void SetSelectPickerSourceVehicless()
        {
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList();
            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
        }
        [HttpGet]
        public IActionResult Vehicles()
        {
            var model = new CatalogViewModel
            {
                DataUrl = Url.Action("GetVehicles")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "registrationNo", title = "Registration No", formatter = "vehicleDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "registrationDate", title = "Registration Date",sorter="dateSorter", formatter = "dateFormatter"},
                new ColumnBootstrapTable{ field = "manufactureDate", title = "Manufacture Date",sorter="dateSorter", formatter = "dateFormatter"},
                new ColumnBootstrapTable{ field = "purchaseDate", title = "Purchase Date",sorter="dateSorter", formatter = "dateFormatter"},
                new ColumnBootstrapTable{ field = "engineNo", title = "Engine No"},
                new ColumnBootstrapTable{ field = "chassisNo", title = "Chassis No"},
                new ColumnBootstrapTable{ field = "brand", title = "Brand"},
                new ColumnBootstrapTable{ field = "model", title = "Model"},
                new ColumnBootstrapTable{ field = "color", title = "Color"},
                new ColumnBootstrapTable{ field = "capacity", title = "Capacity"},
                new ColumnBootstrapTable{ field = "defaultDriver", title = "Default Driver"},
                new ColumnBootstrapTable{ field = "defaultDepartment", title = "Default Department"},
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            SetSelectPickerSourceVehicless();

            return View("/Views/Catalog/Vehicle/Vehicles.cshtml", model);
        }

        public async Task<IActionResult> GetVehicles()
        {
            List<VehicleViewModel> result = new List<VehicleViewModel>();

            var vehicles = _context.Vehicles.Include(v => v.DefaultDepartment).Include(v => v.DefaultDriver)
                .AsQueryable();

            #region Map entity to ViewModel
            await vehicles.ForEachAsync(vehicle =>
            {
                result.Add(new VehicleViewModel
                {
                    Id = vehicle.Id.ToLowerString(),
                    RegistrationNo = vehicle.RegistrationNo,
                    RegistrationDate = vehicle.RegistrationDate,
                    ManufactureDate = vehicle.ManufactureDate,
                    PurchaseDate = vehicle.PurchaseDate,
                    EngineNo = vehicle.EngineNo,
                    ChassisNo = vehicle.ChassisNo,
                    Brand = vehicle.Brand,
                    Model = vehicle.ModelNo,
                    Color = vehicle.Color,
                    Capacity = vehicle.Capacity,
                    DefaultDriverId = vehicle.DefaultDriverId,
                    DefaultDriver = vehicle.DefaultDriver?.FirstName,
                    DefaultDepartmentId = vehicle.DefaultDepartmentId,
                    DefaultDepartment = vehicle.DefaultDepartment?.Name,
                    Note = vehicle.Note,
                    Status = vehicle.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewVehicle()
        {
            var model = new Vehicle
            {
                Id = Guid.NewGuid(),
                RegistrationDate = DateTime.Now,
                PurchaseDate = DateTime.Now,
                ManufactureDate = DateTime.Now,
                Capacity = 4

            };

            SetSelectPickerSourceVehicless();
            return View("/Views/Catalog/Vehicle/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditVehicle(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Vehicles.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            SetSelectPickerSourceVehicless();
            return View("/Views/Catalog/Vehicle/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailVehicle(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Vehicles.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            SetSelectPickerSourceVehicless();
            return View("/Views/Catalog/Vehicle/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveVehicle(int mode, Vehicle model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.Vehicles.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Vehicles.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }
                        model.CurrentVehicleRepairId = existsEntity.CurrentVehicleRepairId;
                        model.CurrentVehicleScheduleId = existsEntity.CurrentVehicleScheduleId;

                        _context.Vehicles.Update(model);
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

                return Json(new { model.Id, name = model.RegistrationNo });
            }

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }

        [HttpPost]
        public IActionResult DeleteVehicle(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Vehicles.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Vehicles.Remove(model);

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

        #region VehicleCosts

        [HttpGet]
        public IActionResult VehicleCosts()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetVehicleCosts")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "vehicleCostDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/VehicleCost/VehicleCosts.cshtml", model);
        }

        public async Task<IActionResult> GetVehicleCosts()
        {
            List<VehicleCostViewModel> result = new List<VehicleCostViewModel>();

            var vehicleCosts = _context.VehicleCosts
                .AsQueryable();

            #region Map entity to ViewModel
            await vehicleCosts.ForEachAsync(vehicleCost =>
            {
                result.Add(new VehicleCostViewModel
                {
                    Id = vehicleCost.Id.ToLowerString(),
                    Name = vehicleCost.Name,
                    Note = vehicleCost.Note,
                    Status = vehicleCost.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewVehicleCost()
        {
            var model = new VehicleCost
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/VehicleCost/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditVehicleCost(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleCosts.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/VehicleCost/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailVehicleCost(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleCosts.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/VehicleCost/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveVehicleCost(int mode, VehicleCost model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.VehicleCosts.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.VehicleCosts.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.VehicleCosts.Update(model);
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
        public IActionResult DeleteVehicleCost(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.VehicleCosts.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.VehicleCosts.Remove(model);

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

        #region VehicleUsePurposes

        [HttpGet]
        public IActionResult VehicleUsePurposes()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetVehicleUsePurposes")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "vehicleUsePurposeDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/VehicleUsePurpose/VehicleUsePurposes.cshtml", model);
        }

        public async Task<IActionResult> GetVehicleUsePurposes()
        {
            List<VehicleUsePurposeViewModel> result = new List<VehicleUsePurposeViewModel>();

            var vehicleUsePurposes = _context.VehicleUsePurposes
                .AsQueryable();

            #region Map entity to ViewModel
            await vehicleUsePurposes.ForEachAsync(vehicleUsePurpose =>
            {
                result.Add(new VehicleUsePurposeViewModel
                {
                    Id = vehicleUsePurpose.Id.ToLowerString(),
                    Name = vehicleUsePurpose.Name,
                    Note = vehicleUsePurpose.Note,
                    Status = vehicleUsePurpose.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewVehicleUsePurpose()
        {
            var model = new VehicleUsePurpose
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/VehicleUsePurpose/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditVehicleUsePurpose(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleUsePurposes.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/VehicleUsePurpose/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailVehicleUsePurpose(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleUsePurposes.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/VehicleUsePurpose/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveVehicleUsePurpose(int mode, VehicleUsePurpose model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.VehicleUsePurposes.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.VehicleUsePurposes.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.VehicleUsePurposes.Update(model);
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
        public IActionResult DeleteVehicleUsePurpose(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.VehicleUsePurposes.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.VehicleUsePurposes.Remove(model);

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

        #region Divisions

        private void SetDivisionsPickerSource()
        {
            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
        }
        [HttpGet]
        public IActionResult Divisions()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetDivisions")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "divisionDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "department", title = "Department" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/Division/Divisions.cshtml", model);
        }

        public async Task<IActionResult> GetDivisions()
        {
            List<DivisionViewModel> result = new List<DivisionViewModel>();

            var divisions = _context.Divisions.Include(d => d.Department)
                .AsQueryable();

            #region Map entity to ViewModel
            await divisions.ForEachAsync(division =>
            {
                result.Add(new DivisionViewModel
                {
                    Id = division.Id.ToLowerString(),
                    Name = division.Name,
                    DepartmentId = division.DepartmentId,
                    Department = division.Department.Name,
                    Note = division.Note,
                    Status = division.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewDivision()
        {
            var model = new Division
            {
                Id = Guid.NewGuid()
            };
            SetDivisionsPickerSource();
            return View("/Views/Catalog/Division/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditDivision(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Divisions.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            SetDivisionsPickerSource();
            return View("/Views/Catalog/Division/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailDivision(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Divisions.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }
            SetDivisionsPickerSource();
            return View("/Views/Catalog/Division/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveDivision(int mode, Division model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.Divisions.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Divisions.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.Divisions.Update(model);
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
        public IActionResult DeleteDivision(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Divisions.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Divisions.Remove(model);

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

        #region Holidays

        [HttpGet]
        public IActionResult Holidays()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetHolidays")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "holidayDrillDownFormatter" },
                new ColumnBootstrapTable { field = "startDate", title = "Start Date", formatter = "dateFormatter", sorter = "dateSorter" },
                new ColumnBootstrapTable { field = "endDate", title = "End Date", formatter = "dateFormatter", sorter = "dateSorter" },
                new ColumnBootstrapTable { field = "numberOfDays", title = "Number Of Days", formatter = "numberFormatterPacking", footerFormatter = "summaryFooterFormatterPacking" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/Holiday/Holidays.cshtml", model);
        }

        public async Task<IActionResult> GetHolidays()
        {
            List<HolidayViewModel> result = new List<HolidayViewModel>();

            var holidays = _context.Holidays
                .AsQueryable();

            #region Map entity to ViewModel
            await holidays.ForEachAsync(holiday =>
            {
                result.Add(new HolidayViewModel
                {
                    Id = holiday.Id.ToLowerString(),
                    Name = holiday.Name,
                    StartDate = holiday.StartDate,
                    EndDate = holiday.EndDate,
                    NumberOfDays = holiday.NumberOfDays,
                    Note = holiday.Note,
                    Status = holiday.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewHoliday()
        {
            var model = new Holiday
            {
                Id = Guid.NewGuid(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            return View("/Views/Catalog/Holiday/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditHoliday(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Holidays.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Holiday/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailHoliday(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Holidays.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Holiday/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveHoliday(int mode, Holiday model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.Holidays.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Holidays.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.Holidays.Update(model);
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
        public IActionResult DeleteHoliday(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Holidays.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Holidays.Remove(model);

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

        #region Positions

        [HttpGet]
        public IActionResult Positions()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetPositions")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "positionDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/Position/Positions.cshtml", model);
        }

        public async Task<IActionResult> GetPositions()
        {
            List<PositionViewModel> result = new List<PositionViewModel>();

            var positions = _context.Positions
                .AsQueryable();

            #region Map entity to ViewModel
            await positions.ForEachAsync(position =>
            {
                result.Add(new PositionViewModel
                {
                    Id = position.Id.ToLowerString(),
                    Name = position.Name,
                    Note = position.Note,
                    Status = position.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewPosition()
        {
            var model = new Position
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/Position/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditPosition(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Positions.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Position/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailPosition(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.Positions.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/Position/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SavePosition(int mode, Position model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.Positions.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.Positions.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.Positions.Update(model);
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
        public IActionResult DeletePosition(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.Positions.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Positions.Remove(model);

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

        #region LeaveTypes

        [HttpGet]
        public IActionResult LeaveTypes()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetLeaveTypes")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "leaveTypeDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/LeaveType/LeaveTypes.cshtml", model);
        }

        public async Task<IActionResult> GetLeaveTypes()
        {
            List<LeaveTypeViewModel> result = new List<LeaveTypeViewModel>();

            var leaveTypes = _context.LeaveTypes
                .AsQueryable();

            #region Map entity to ViewModel
            await leaveTypes.ForEachAsync(leaveType =>
            {
                result.Add(new LeaveTypeViewModel
                {
                    Id = leaveType.Id.ToLowerString(),
                    Name = leaveType.Name,
                    Note = leaveType.Note,
                    Status = leaveType.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewLeaveType()
        {
            var model = new LeaveType
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/LeaveType/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditLeaveType(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.LeaveTypes.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/LeaveType/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailLeaveType(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.LeaveTypes.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/LeaveType/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveLeaveType(int mode, LeaveType model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.LeaveTypes.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.LeaveTypes.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.LeaveTypes.Update(model);
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
        public IActionResult DeleteLeaveType(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.LeaveTypes.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.LeaveTypes.Remove(model);

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

        #region ContractTypes

        [HttpGet]
        public IActionResult ContractTypes()
        {
            var model = new CatalogViewModel
            {

                DataUrl = Url.Action("GetContractTypes")
            };

            model.Columns = new List<Helpers.ColumnBootstrapTable> {
                new ColumnBootstrapTable{ field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                new ColumnBootstrapTable{ field = "name", title = "Name", formatter = "contractTypeDrillDownFormatter" },
                new ColumnBootstrapTable{ field = "note", title = "Note" },
                new ColumnBootstrapTable{ field = "status", title = "Status", width = "30", formatter = "statusFormatter" }
            };

            return View("/Views/Catalog/ContractType/ContractTypes.cshtml", model);
        }

        public async Task<IActionResult> GetContractTypes()
        {
            List<ContractTypeViewModel> result = new List<ContractTypeViewModel>();

            var contractTypes = _context.ContractTypes
                .AsQueryable();

            #region Map entity to ViewModel
            await contractTypes.ForEachAsync(contractType =>
            {
                result.Add(new ContractTypeViewModel
                {
                    Id = contractType.Id.ToLowerString(),
                    Name = contractType.Name,
                    Note = contractType.Note,
                    Status = contractType.Status.ToString(),
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult NewContractType()
        {
            var model = new ContractType
            {
                Id = Guid.NewGuid()
            };

            return View("/Views/Catalog/ContractType/New.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditContractType(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.ContractTypes.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/ContractType/Edit.cshtml", model);
        }

        [HttpGet]
        public IActionResult DetailContractType(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.ContractTypes.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            return View("/Views/Catalog/ContractType/Detail.cshtml", model);
        }

        [HttpPost]
        public IActionResult SaveContractType(int mode, ContractType model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (mode == 1)
                    {
                        _context.ContractTypes.Add(model);
                    }
                    else
                    {
                        var existsEntity = _context.ContractTypes.AsNoTracking().First(c => c.Id == model.Id);

                        if (existsEntity == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"{model.GetType().Name} not found");
                        }

                        _context.ContractTypes.Update(model);
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
        public IActionResult DeleteContractType(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.ContractTypes.FirstOrDefault(c => c.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.ContractTypes.Remove(model);

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
    }
}
