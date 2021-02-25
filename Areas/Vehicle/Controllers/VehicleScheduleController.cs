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

namespace Develover.WebUI.Controllers
{
    [Area("Vehicle")]
    [Route("vehicle/schedule/{action=index}")]
    public class VehicleScheduleController : Controller
    {
        private readonly object _savelock = new object();
        private readonly ILogger<VehicleScheduleController> _logger;
        private readonly DataContext _context;

        private static List<ColumnBootstrapTable> DetailColumns => new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "vehicleScheduleId", visible = false },
                    new ColumnBootstrapTable { field = "sequeceNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "issueDate", title = "Issue Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "vehicleCostId", visible = false },
                    new ColumnBootstrapTable { field = "vehicleCost", title = "Vehicle" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "price", title = "Price", formatter = "numberFormatterAmount", footerFormatter = "summaryFooterFormatterAmount" },
                    new ColumnBootstrapTable { field = "amount", title = "Amount", formatter = "numberFormatterAmountVND", footerFormatter = "summaryFooterFormatterAmountVND" },
                    new ColumnBootstrapTable { field = "note", title = "Note"},
                    new ColumnBootstrapTable { field = "operate", width = "50", formatter = "operateFormatter", events = "window.operateEvents", clickToSelect = false, sortable = false }
                };

        public VehicleScheduleController(ILogger<VehicleScheduleController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        private void SetSelectPickerSource()
        {
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList();
            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Vehicles"] = _context.Vehicles.OrderBy(c => c.RegistrationNo).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.RegistrationNo }).ToList();
            ViewData["VehicleCosts"] = _context.VehicleCosts.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["VehicleUsePurposes"] = _context.VehicleUsePurposes.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
        }

        public IActionResult Index()
        {
            var model = new VoucherViewModel
            {
                DataUrl = "/vehicle/schedule/getvehicleschedules",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "issueDate", title = "Issue Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "no", title = "No", formatter = "vehicleScheduleDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "departureDate", title = "Departure Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "returnDate", title = "Return Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "vehicleId", visible = false },
                    new ColumnBootstrapTable { field = "vehicle", title = "Vehicle" },
                    new ColumnBootstrapTable { field = "driverId", visible = false },
                    new ColumnBootstrapTable { field = "driver", title = "Driver" },
                    new ColumnBootstrapTable { field = "petitionerId", visible = false },
                    new ColumnBootstrapTable { field = "petitioner", title = "Petitioner" },
                    new ColumnBootstrapTable { field = "departmentId", visible = false },
                    new ColumnBootstrapTable { field = "department", title = "Department" },
                    new ColumnBootstrapTable { field = "origin", title = "Origin" },
                    new ColumnBootstrapTable { field = "destination", title = "Destination" },
                    new ColumnBootstrapTable { field = "vehicleUsePurposeId", visible = false },
                    new ColumnBootstrapTable { field = "vehicleUsePurpose", title = "Use Purpose" },
                    new ColumnBootstrapTable { field = "totalQuantity", title = "Total Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "totalAmount", title = "Total Amount", formatter = "numberFormatterAmountVND", footerFormatter = "summaryFooterFormatterAmountVND" },
                    new ColumnBootstrapTable { field = "note", title = "Note" }
                }
            };
            return View(model);
        }

        public async Task<IActionResult> GetVehicleSchedules()
        {
            List<VehicleScheduleViewModel> result = new List<VehicleScheduleViewModel>();

            var issueNotes = _context.VehicleSchedules
                .Include(schedule => schedule.Vehicle)
                .Include(schedule => schedule.Driver)
                .Include(schedule => schedule.Petitioner)
                .Include(schedule => schedule.Department)
                .Include(schedule => schedule.UsePurpose)
                .Include(schedule => schedule.Items)
                .AsQueryable();

            #region Map entity to ViewModel
            await issueNotes.ForEachAsync(voucher =>
            {
                result.Add(new VehicleScheduleViewModel
                {
                    Id = voucher.Id,
                    IssueDate = voucher.IssueDate,
                    No = voucher.No,
                    DepartureDate = voucher.DepartureDate,
                    ReturnDate = voucher.ReturnDate,
                    VehicleId = voucher.VehicleId,
                    Vehicle = voucher.Vehicle.RegistrationNo,
                    DriverId = voucher.DriverId,
                    Driver = voucher.Driver.FirstName,
                    PetitionerId = voucher.PetitionerId,
                    Petitioner = voucher.Petitioner?.FirstName,
                    DepartmentId = voucher.DepartmentId,
                    Department = voucher.Department?.Name,
                    Origin = voucher.Origin,
                    Destination = voucher.Destination,
                    VehicleUsePurposeId = voucher.VehicleUsePurposeId,
                    UsePurpose = voucher.UsePurpose.Name,
                    TotalQuantity = voucher.TotalQuantity,
                    TotalAmount = voucher.TotalAmount,
                    Note = voucher.Note,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        public async Task<IActionResult> GetVehicleScheduleItems(string id)
        {
            List<VehicleScheduleItemViewModel> result = new List<VehicleScheduleItemViewModel>();

            _ = Guid.TryParse(id, out Guid modelId);

            var items = _context.VehicleScheduleItems
                .Include(i => i.VehicleSchedule)
                .Include(i => i.VehicleCost)
                .Where(i => i.VehicleScheduleId == modelId)
                .AsQueryable();

            #region Map entity to ViewModel
            await items.ForEachAsync(item =>
            {
                result.Add(new VehicleScheduleItemViewModel
                {
                    Id = item.Id,
                    VehicleScheduleId = item.VehicleScheduleId,
                    IssueDate = item.IssueDate,
                    VehicleCostId = item.VehicleCostId,
                    VehicleCost = item.VehicleCost.Name,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Amount = item.Amount,
                    Note = item.Note,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult New()
        {
            var model = new VehicleSchedule
            {
                Id = Guid.NewGuid(),
                IssueDate = DateTime.Now,
                DepartureDate = DateTime.Now,
                ReturnDate = DateTime.Now,
                Items = new List<VehicleScheduleItem>(),
                DetailDataUrl = $"/vehicle/schedule/getvehiclescheduleitems?id={Guid.Empty}",
                DetailColumns = DetailColumns
            };

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleSchedules.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/vehicle/schedule/getvehiclescheduleitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleSchedules.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/vehicle/schedule/getvehiclescheduleitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(int mode, VehicleSchedule model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    lock (_savelock)
                    {
                        if (!model.Complete)
                        {
                            var modelSchedules = _context.VehicleSchedules.AsNoTracking().Where(s =>s.Id != model.Id && s.VehicleId == model.VehicleId && ((s.DepartureDate >= model.DepartureDate && s.ReturnDate <= model.ReturnDate) || (s.DepartureDate <= model.DepartureDate && s.ReturnDate >= model.ReturnDate) || (s.DepartureDate <= model.DepartureDate && s.ReturnDate >= model.DepartureDate) || (s.DepartureDate <= model.ReturnDate && s.ReturnDate >= model.ReturnDate))).FirstOrDefault();
                            if (modelSchedules != null)
                            {
                                Response.StatusCode = 404;
                                return Json($"The schedule has been coincided, please check again?");
                            }
                            
                        }

                        if (mode == 1)
                        {
                            if (!model.Complete)
                            {
                                if (model.DepartureDate < DateTime.Now || model.ReturnDate < DateTime.Now)
                                {
                                    Response.StatusCode = 404;
                                    return Json($"Please select a date greater than or equal to the current date");
                                }
                            }

                            model.No = GetNoVoucher(model.Id);
                            model.SequenceNo = _context.VehicleSchedules.OrderByDescending(d => d.SequenceNo).FirstOrDefault()?.SequenceNo + 1 ?? 1;
                            _context.VehicleSchedules.Add(model);
                        }
                        else
                        {
                            var existsEntity = _context.VehicleSchedules.AsNoTracking().First(v => v.Id == model.Id);

                            if (existsEntity == null)
                            {
                                Response.StatusCode = 404;
                                return Json($"{model.GetType().Name} not found");
                            }

                            _context.VehicleScheduleItems.RemoveRange(_context.VehicleScheduleItems.Where(i => i.VehicleScheduleId == model.Id));
                            _context.VehicleScheduleItems.AddRange(model.Items);
                            _context.VehicleSchedules.Update(model);

                            if (model.Complete)
                            {
                                var vehicle = _context.Vehicles.Where(v => v.Id == model.VehicleId && v.CurrentVehicleScheduleId == model.Id).FirstOrDefault();
                                if (vehicle != null)
                                {
                                    vehicle.IsDriving=false;
                                    vehicle.CurrentVehicleScheduleId = null;
                                    _context.Vehicles.Update(vehicle);
                                }
                            }
                        }

                        _context.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.RollbackAsync();
                    Response.StatusCode = 500;
                    return Json(ex.InnerException.Message);
                }

                return Json(new { model.Id });
            }

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = _context.VehicleSchedules.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.VehicleScheduleItems.RemoveRange(model.Items);
                _context.VehicleSchedules.Remove(model);

                _context.SaveChanges();
                transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json(ex.Message);
            }

            return Ok();
        }

        public string GetNoVoucher(Guid id)
        {
            var len = 3;
            var structureNo = "{YYYY}/{MM}/-{NO}";
            var typeNo = 1;//1. follow year 2. follow month 3. follow day 0 up to 
            var entityDeveloverSetting = _context.DeveloverSettings.FirstOrDefault();

            if (entityDeveloverSetting != null)
            {
                len = entityDeveloverSetting.LenghtNo;
                structureNo = entityDeveloverSetting.StructureNo;
                typeNo = entityDeveloverSetting.TypeNo;
            }

            var entity = _context.VehicleSchedules.AsNoTracking().Where(v => v.Id != id);

            switch (typeNo)
            {
                case 1:
                    entity = entity.Where(v => v.IssueDate.Year == DateTime.Now.Year);
                    break;
                case 2:
                    entity = entity.Where(v => v.IssueDate.Year == DateTime.Now.Year && v.IssueDate.Month == DateTime.Now.Month);
                    break;
                case 3:
                    entity = entity.Where(v => v.IssueDate.Year == DateTime.Now.Year && v.IssueDate.Month == DateTime.Now.Month && v.IssueDate.Day == DateTime.Now.Day);
                    break;
                default:
                    break;
            }

            var sttNo = entity.OrderByDescending(v => v.SequenceNo).Select(v => v.No).FirstOrDefault();

            return sttNo.CreateNoByStructureNo(len, structureNo);

        }
        [HttpGet]
        public IActionResult GetNo(string id)
        {
            _ = Guid.TryParse(id, out Guid Id);
            var no = GetNoVoucher(Id);
            return Json(new { no });
        }
    }
}
