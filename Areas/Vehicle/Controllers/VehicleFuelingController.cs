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
    [Route("vehicle/fueling/{action=index}")]
    public class VehicleFuelingController : Controller
    {
        private readonly object _savelock = new object();
        private readonly ILogger<VehicleFuelingController> _logger;
        private readonly DataContext _context;

        private static List<ColumnBootstrapTable> DetailColumns => new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "vehicleFuelingId", visible = false },
                    new ColumnBootstrapTable { field = "sequeceNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "vehicleId", visible = false },
                    new ColumnBootstrapTable { field = "vehicle", title = "Vehicle" },
                    new ColumnBootstrapTable { field = "receiverId", visible = false },
                    new ColumnBootstrapTable { field = "receiver", title = "Receiver" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "currentKM", title = "Current KM", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "note", title = "Note"},
                    new ColumnBootstrapTable { field = "operate", width = "50", formatter = "operateFormatter", events = "window.operateEvents", clickToSelect = false, sortable = false }
                };

        public VehicleFuelingController(ILogger<VehicleFuelingController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        private void SetSelectPickerSource()
        {
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList();
            ViewData["Vehicles"] = _context.Vehicles.OrderBy(c => c.RegistrationNo).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.RegistrationNo }).ToList();
        }

        public IActionResult Index()
        {
            var model = new VoucherViewModel
            {
                DataUrl = "/vehicle/fueling/getvehiclefuelings",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "issueDate", title = "Issue Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "no", title = "No", formatter = "vehicleFuelingDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "issuerId", visible = false },
                    new ColumnBootstrapTable { field = "issuer", title = "Issuer" },
                    new ColumnBootstrapTable { field = "totalQuantity", title = "Total Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "note", title = "Note" }
                }
            };
            return View(model);
        }

        public async Task<IActionResult> GetVehicleFuelings()
        {
            List<VehicleFuelingViewModel> result = new List<VehicleFuelingViewModel>();

            var issueNotes = _context.VehicleFuelings
                .Include(fueling => fueling.Issuer)
                .Include(fueling => fueling.Items)
                .AsQueryable();

            #region Map entity to ViewModel
            await issueNotes.ForEachAsync(voucher =>
            {
                result.Add(new VehicleFuelingViewModel
                {
                    Id = voucher.Id,
                    IssueDate = voucher.IssueDate,
                    No = voucher.No,
                    IssuerId = voucher.IssuerId,
                    Issuer = voucher.Issuer.FirstName,
                    TotalQuantity = voucher.TotalQuantity,
                    Note = voucher.Note,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        public async Task<IActionResult> GetVehicleFuelingItems(string id)
        {
            List<VehicleFuelingItemViewModel> result = new List<VehicleFuelingItemViewModel>();

            _ = Guid.TryParse(id, out Guid modelId);

            var items = _context.VehicleFuelingItems
                .Include(i => i.VehicleFueling)
                    .ThenInclude(i => i.Issuer)
                .Include(i => i.Vehicle)
                .Include(i => i.Receiver)
                .Where(i => i.VehicleFuelingId == modelId)
                .AsQueryable();

            #region Map entity to ViewModel
            await items.ForEachAsync(item =>
            {
                result.Add(new VehicleFuelingItemViewModel
                {
                    Id = item.Id,
                    VehicleFuelingId = item.VehicleFuelingId,
                    VehicleId = item.VehicleId,
                    Vehicle = item.Vehicle.RegistrationNo,
                    ReceiverId = item.ReceiverId,
                    Receiver = item.Receiver.FirstName,
                    Quantity = item.Quantity,
                    CurrentKM = item.CurrentKM,
                    Note = item.Note,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult New()
        {
            var model = new VehicleFueling
            {
                Id = Guid.NewGuid(),
                IssueDate = DateTime.Now,
                Items = new List<VehicleFuelingItem>(),
                DetailDataUrl = $"/vehicle/fueling/getvehiclefuelingitems?id={Guid.Empty}",
                DetailColumns = DetailColumns
            };

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleFuelings.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/vehicle/fueling/getvehiclefuelingitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.VehicleFuelings.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/vehicle/fueling/getvehiclefuelingitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(int mode, VehicleFueling model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    lock (_savelock)
                    {
                        if (mode == 1)
                        {
                            model.No = GetNoVoucher(model.Id);
                            model.SequenceNo = _context.VehicleFuelings.OrderByDescending(d => d.SequenceNo).FirstOrDefault()?.SequenceNo + 1 ?? 1;
                            _context.VehicleFuelings.Add(model);
                        }
                        else
                        {
                            var existsEntity = _context.VehicleFuelings.AsNoTracking().First(v => v.Id == model.Id);

                            if (existsEntity == null)
                            {
                                Response.StatusCode = 404;
                                return Json($"{model.GetType().Name} not found");
                            }

                            _context.VehicleFuelingItems.RemoveRange(_context.VehicleFuelingItems.Where(i => i.VehicleFuelingId == model.Id));
                            _context.VehicleFuelingItems.AddRange(model.Items);
                            _context.VehicleFuelings.Update(model);
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
            var model = _context.VehicleFuelings.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.VehicleFuelingItems.RemoveRange(model.Items);
                _context.VehicleFuelings.Remove(model);

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

        [HttpGet]
        public IActionResult GetDefaultDriverOfVehicle(string id)
        {
            _ = Guid.TryParse(id, out Guid vehicleId);
            var defaultDriver = _context.Vehicles.Include(x => x.DefaultDriver).FirstOrDefault(x => x.Id == vehicleId).DefaultDriver;

            return Json(defaultDriver);
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

            var entity = _context.VehicleFuelings.AsNoTracking().Where(v => v.Id != id);

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
