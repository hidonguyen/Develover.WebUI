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
    [Route("vehicle/repair/{action=index}")]
    public class VehicleRepairController : Controller
    {
        private readonly object _savelock = new object();
        private readonly ILogger<VehicleRepairController> _logger;
        private readonly DataContext _context;

        private static List<ColumnBootstrapTable> DetailColumns => new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "vehicleRepairId", visible = false },
                    new ColumnBootstrapTable { field = "sequeceNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "vehicleCostId", visible = false },
                    new ColumnBootstrapTable { field = "vehicleCost", title = "Vehicle" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "price", title = "Price", formatter = "numberFormatterAmount", footerFormatter = "summaryFooterFormatterAmount" },
                    new ColumnBootstrapTable { field = "amount", title = "Amount", formatter = "numberFormatterAmountVND", footerFormatter = "summaryFooterFormatterAmountVND" },
                    new ColumnBootstrapTable { field = "note", title = "Note"},
                    new ColumnBootstrapTable { field = "operate", width = "50", formatter = "operateFormatter", events = "window.operateEvents", clickToSelect = false, sortable = false }
                };

        public VehicleRepairController(ILogger<VehicleRepairController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        private void SetSelectPickerSource()
        {
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList();
            ViewData["Vehicles"] = _context.Vehicles.OrderBy(c => c.RegistrationNo).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.RegistrationNo }).ToList();
            ViewData["VehicleCosts"] = _context.VehicleCosts.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
        }

        public IActionResult Index()
        {
            var model = new VoucherViewModel
            {
                DataUrl = "/vehicle/repair/getvehiclerepairs",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "issueDate", title = "Issue Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "no", title = "No", formatter = "vehicleRepairDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "vehicleId", visible = false },
                    new ColumnBootstrapTable { field = "vehicle", title = "Vehicle" },
                    new ColumnBootstrapTable { field = "driverId", visible = false },
                    new ColumnBootstrapTable { field = "driver", title = "Driver" },
                    new ColumnBootstrapTable { field = "repairShop", title = "Repair Shop" },
                    new ColumnBootstrapTable { field = "estimatedCompletionDate", title = "Completion Date" },
                    new ColumnBootstrapTable { field = "estimatedRepairCost", title = "Repair Cost" },
                    new ColumnBootstrapTable { field = "totalQuantity", title = "Total Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "totalAmount", title = "Total Amount", formatter = "numberFormatterAmountVND", footerFormatter = "summaryFooterFormatterAmountVND" },
                    new ColumnBootstrapTable { field = "note", title = "Note" }
                }
            };
            return View(model);
        }

        public async Task<IActionResult> GetVehicleRepairs()
        {
            List<VehicleRepairViewModel> result = new List<VehicleRepairViewModel>();

            var issueNotes = _context.VehicleRepairs
                .Include(repair => repair.Vehicle)
                .Include(repair => repair.Driver)
                .Include(repair => repair.Items)
                .AsQueryable();

            #region Map entity to ViewModel
            await issueNotes.ForEachAsync(voucher =>
            {
                result.Add(new VehicleRepairViewModel
                {
                    Id = voucher.Id,
                    IssueDate = voucher.IssueDate,
                    No = voucher.No,
                    VehicleId = voucher.VehicleId,
                    Vehicle = voucher.Vehicle.RegistrationNo,
                    DriverId = voucher.DriverId,
                    Driver = voucher.Driver.FirstName,
                    RepairShop = voucher.RepairShop,
                    EstimatedCompletionDate = voucher.EstimatedCompletionDate,
                    EstimatedRepairCost = voucher.EstimatedRepairCost,
                    TotalQuantity = voucher.TotalQuantity,
                    TotalAmount = voucher.TotalAmount,
                    Note = voucher.Note,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        public async Task<IActionResult> GetVehicleRepairItems(string id)
        {
            List<VehicleRepairItemViewModel> result = new List<VehicleRepairItemViewModel>();

            _ = Guid.TryParse(id, out Guid modelId);

            var items = _context.VehicleRepairItems
                .Include(i => i.VehicleRepair)
                .Include(i => i.VehicleCost)
                .Where(i => i.VehicleRepairId == modelId)
                .AsQueryable();

            #region Map entity to ViewModel
            await items.ForEachAsync(item =>
            {
                result.Add(new VehicleRepairItemViewModel
                {
                    Id = item.Id,
                    VehicleRepairId = item.VehicleRepairId,
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
            var model = new VehicleRepair
            {
                Id = Guid.NewGuid(),
                IssueDate = DateTime.Now,
                EstimatedCompletionDate = DateTime.Now,
                Items = new List<VehicleRepairItem>(),
                DetailDataUrl = $"/vehicle/repair/getvehiclerepairitems?id={Guid.Empty}",
                DetailColumns = DetailColumns
            };

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleRepairs.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/vehicle/repair/getvehiclerepairitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleRepairs.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/vehicle/repair/getvehiclerepairitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(int mode, VehicleRepair model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    if (!model.Complete)
                    {
                        if (_context.VehicleRepairs.AsNoTracking().Where(x =>x.Id != model.Id &&  x.VehicleId == model.VehicleId && x.Id == model.Id && x.Complete == false ).Any())
                        {
                            Response.StatusCode = 404;
                            return Json($"The vehicle is repairing, you cannot take the car for repair");
                        }
                    }

                    lock (_savelock)
                    {
                        if (mode == 1)
                        {
                            model.No = GetNoVoucher(model.Id);
                            model.SequenceNo = _context.VehicleRepairs.OrderByDescending(d => d.SequenceNo).FirstOrDefault()?.SequenceNo + 1 ?? 1;

                            if (!model.Complete)
                            {
                                var vehicle = _context.Vehicles.Where(v => v.Id == model.VehicleId).FirstOrDefault();
                                if (vehicle != null)
                                {
                                    vehicle.IsRepairing = true;
                                    vehicle.CurrentVehicleRepairId = model.Id;
                                    _context.Vehicles.Update(vehicle);
                                }
                            }
                            _context.VehicleRepairs.Add(model);
                        }
                        else
                        {
                            var existsEntity = _context.VehicleRepairs.AsNoTracking().First(v => v.Id == model.Id);

                            if (existsEntity == null)
                            {
                                Response.StatusCode = 404;
                                return Json($"{model.GetType().Name} not found");
                            }

                            _context.VehicleRepairItems.RemoveRange(_context.VehicleRepairItems.Where(i => i.VehicleRepairId == model.Id));
                            _context.VehicleRepairItems.AddRange(model.Items);
                            _context.VehicleRepairs.Update(model);

                            if (model.Complete)
                            {
                                var vehicle = _context.Vehicles.Where(v => v.Id == model.VehicleId && v.CurrentVehicleRepairId == model.Id).FirstOrDefault();
                                if (vehicle != null)
                                {
                                    vehicle.IsRepairing = false;
                                    vehicle.CurrentVehicleRepairId = null;
                                    _context.Vehicles.Update(vehicle);
                                }
                            }
                            else
                            {
                                var vehicle = _context.Vehicles.Where(v => v.Id == model.VehicleId).FirstOrDefault();
                                if (vehicle != null)
                                {
                                    vehicle.IsRepairing = true;
                                    vehicle.CurrentVehicleRepairId = model.Id;
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
            var model = _context.VehicleRepairs.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.VehicleRepairItems.RemoveRange(model.Items);
                _context.VehicleRepairs.Remove(model);

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

            var entity = _context.VehicleRepairs.AsNoTracking().Where(v => v.Id != id);

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
