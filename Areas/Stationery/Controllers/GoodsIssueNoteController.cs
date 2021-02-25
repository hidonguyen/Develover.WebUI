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
    [Area("Stationery")]
    [Route("stationery/issue-note/{action=index}")]
    public class GoodsIssueNoteController : Controller
    {
        private readonly object _savelock = new object();
        private readonly ILogger<GoodsIssueNoteController> _logger;
        private readonly DataContext _context;

        private static List<ColumnBootstrapTable> DetailColumns => new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "goodsIssueNoteId", visible = false },
                    new ColumnBootstrapTable { field = "sequeceNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "stockItemId", visible = false },
                    new ColumnBootstrapTable { field = "stockItem", title = "Item" },
                    new ColumnBootstrapTable { field = "unitOfMeasureId", visible = false },
                    new ColumnBootstrapTable { field = "unitOfMeasure", title = "UoM" },
                    new ColumnBootstrapTable { field = "locationId", visible = false },
                    new ColumnBootstrapTable { field = "location", title = "Location" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "note", title = "Note"},
                    new ColumnBootstrapTable { field = "operate", width = "50", formatter = "operateFormatter", events = "window.operateEvents", clickToSelect = false, sortable = false }
                };

        public GoodsIssueNoteController(ILogger<GoodsIssueNoteController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        private void SetSelectPickerSource()
        {
            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList();
            ViewData["StockItems"] = _context.StockItems.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Locations"] = _context.Locations.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();


        }

        public IActionResult Index()
        {
            var model = new VoucherViewModel
            {
                DataUrl = "/stationery/issue-note/getgoodsissuenotes",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "date", title = "Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "no", title = "No", formatter = "goodsIssueNoteDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "departmentId", visible = false },
                    new ColumnBootstrapTable { field = "department", title = "Department" },
                    new ColumnBootstrapTable { field = "receiverId", visible = false },
                    new ColumnBootstrapTable { field = "receiver", title = "Receiver" },
                    new ColumnBootstrapTable { field = "totalQuantity", title = "Total Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "note", title = "Note" }
                }
            };
            return View(model);
        }

        public async Task<IActionResult> GetGoodsIssueNotes()
        {
            List<GoodsIssueNoteViewModel> result = new List<GoodsIssueNoteViewModel>();

            var issueNotes = _context.GoodsIssueNotes
                .Include(issueNote => issueNote.Department)
                .Include(issueNote => issueNote.Receiver)
                .Include(issueNote => issueNote.Items)
                .AsQueryable();

            #region Map entity to ViewModel
            await issueNotes.ForEachAsync(voucher =>
            {
                result.Add(new GoodsIssueNoteViewModel
                {
                    Id = voucher.Id,
                    Date = voucher.Date,
                    No = voucher.No,
                    DepartmentId = voucher.DepartmentId,
                    Department = voucher.Department.Name,
                    ReceiverId = voucher.ReceiverId,
                    Receiver = voucher.Receiver.FirstName,
                    TotalQuantity = voucher.TotalQuantity,
                    Note = voucher.Note,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        public async Task<IActionResult> GetGoodsIssueNoteItems(string id)
        {
            List<GoodsIssueNoteItemViewModel> result = new List<GoodsIssueNoteItemViewModel>();

            _ = Guid.TryParse(id, out Guid modelId);

            var items = _context.GoodsIssueNoteItems
                .Include(i => i.StockItem)
                    .ThenInclude(i => i.UnitOfMeasure)
                .Include(i => i.Location)
                .Where(i => i.GoodsIssueNoteId == modelId)
                .OrderBy(i => i.SequenceNo)
                .AsQueryable();

            #region Map entity to ViewModel
            await items.ForEachAsync(item =>
            {
                result.Add(new GoodsIssueNoteItemViewModel
                {
                    Id = item.Id,
                    GoodsIssueNoteId = item.GoodsIssueNoteId,
                    StockItemId = item.StockItemId,
                    StockItem = item.StockItem.Name,
                    UnitOfMeasureId = item.StockItem.UnitOfMeasureId,
                    UnitOfMeasure = item.StockItem.UnitOfMeasure.Name,
                    LocationId = item.LocationId,
                    Location = item.Location.Name,
                    Quantity = item.Quantity,
                    Note = item.Note,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        [HttpGet]
        public IActionResult New()
        {
            var model = new GoodsIssueNote
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                Items = new List<GoodsIssueNoteItem>(),
                DetailDataUrl = $"/stationery/issue-note/getgoodsissuenoteitems?id={Guid.Empty}",
                DetailColumns = DetailColumns
            };

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.GoodsIssueNotes.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/stationery/issue-note/getgoodsissuenoteitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.GoodsIssueNotes.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/stationery/issue-note/getgoodsissuenoteitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(int mode, GoodsIssueNote model)
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
                            model.SequenceNo = _context.GoodsIssueNotes.OrderByDescending(d => d.SequenceNo).FirstOrDefault()?.SequenceNo + 1 ?? 1;
                            _context.GoodsIssueNotes.Add(model);
                        }
                        else
                        {
                            var existsEntity = _context.GoodsIssueNotes.AsNoTracking().First(v => v.Id == model.Id);

                            if (existsEntity == null)
                            {
                                Response.StatusCode = 404;
                                return Json($"{model.GetType().Name} not found");
                            }

                            _context.GoodsIssueNoteItems.RemoveRange(_context.GoodsIssueNoteItems.Where(i => i.GoodsIssueNoteId == model.Id));
                            _context.GoodsIssueNoteItems.AddRange(model.Items);
                            _context.GoodsIssueNotes.Update(model);
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
            var model = _context.GoodsIssueNotes.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.GoodsIssueNoteItems.RemoveRange(model.Items);
                _context.GoodsIssueNotes.Remove(model);

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
        public IActionResult GetUoMofItem(string id)
        {
            _ = Guid.TryParse(id, out Guid stockItemId);
            var unitOfMeasure = _context.StockItems.Include(x => x.UnitOfMeasure).FirstOrDefault(x => x.Id == stockItemId).UnitOfMeasure;

            return Json(unitOfMeasure);
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

            var entity = _context.GoodsIssueNotes.AsNoTracking().Where(v => v.Id != id);

            switch (typeNo)
            {
                case 1:
                    entity = entity.Where(v => v.Date.Year == DateTime.Now.Year);
                    break;
                case 2:
                    entity = entity.Where(v => v.Date.Year == DateTime.Now.Year && v.Date.Month == DateTime.Now.Month);
                    break;
                case 3:
                    entity = entity.Where(v => v.Date.Year == DateTime.Now.Year && v.Date.Month == DateTime.Now.Month && v.Date.Day == DateTime.Now.Day);
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
