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
    [Route("stationery/receipt-note/{action=index}")]
    public class GoodsReceiptNoteController : Controller
    {
        private readonly object _savelock = new object();
        private readonly ILogger<GoodsReceiptNoteController> _logger;
        private readonly DataContext _context;

        private static List<ColumnBootstrapTable> DetailColumns => new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "goodsReceiptNoteId", visible = false },
                    new ColumnBootstrapTable { field = "sequeceNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "stockItemId", visible = false },
                    new ColumnBootstrapTable { field = "stockItem", title = "Item" },
                    new ColumnBootstrapTable { field = "unitOfMeasureId", visible = false },
                    new ColumnBootstrapTable { field = "unitOfMeasure", title = "UoM" },
                    new ColumnBootstrapTable { field = "locationId", visible = false },
                    new ColumnBootstrapTable { field = "location", title = "Location" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "price", title = "Price", formatter = "numberFormatterAmount", footerFormatter = "summaryFooterFormatterAmount" },
                    new ColumnBootstrapTable { field = "amount", title = "Amount", formatter = "numberFormatterAmountVND", footerFormatter = "summaryFooterFormatterAmountVND" },
                    new ColumnBootstrapTable { field = "note", title = "Note"},
                    new ColumnBootstrapTable { field = "operate", width = "50", formatter = "operateFormatter", events = "window.operateEvents", clickToSelect = false, sortable = false }
                };

        public GoodsReceiptNoteController(ILogger<GoodsReceiptNoteController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> GetList()
        {
            List<GoodsReceiptNoteViewModel> result = new List<GoodsReceiptNoteViewModel>();

            var receiptNotes = _context.GoodsReceiptNotes
                .Include(receiptNote => receiptNote.Supplier)
                .Include(receiptNote => receiptNote.Items)
                .AsQueryable();

            #region Map entity to ViewModel
            await receiptNotes.ForEachAsync(voucher =>
            {
                result.Add(new GoodsReceiptNoteViewModel
                {
                    Id = voucher.Id,
                    Date = voucher.Date,
                    No = voucher.No,
                    SupplierId = voucher.SupplierId,
                    Supplier = voucher.Supplier.Name,
                    TotalQuantity = voucher.TotalQuantity,
                    TotalAmount = voucher.TotalAmount,
                    Note = voucher.Note,
                    Status = voucher.Status,
                });
            });
            #endregion

            return Json(new { data = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetModel(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);
            var model = await _context.GoodsReceiptNotes.FindAsync(modelId);

            if (model == null)
            {
                model = new GoodsReceiptNote();
            }

            return Json(new { model });
        }

        public async Task<IActionResult> GetModelItems(string id)
        {
            List<GoodsReceiptNoteItemViewModel> result = new List<GoodsReceiptNoteItemViewModel>();

            _ = Guid.TryParse(id, out Guid modelId);

            var items = _context.GoodsReceiptNoteItems
                .Include(i => i.StockItem)
                    .ThenInclude(i => i.UnitOfMeasure)
                .Include(i => i.Location)
                .OrderBy(i => i.SequenceNo)
                .Where(i => i.GoodsReceiptNoteId == modelId)
                .AsQueryable();

            #region Map entity to ViewModel
            await items.ForEachAsync(item =>
            {
                result.Add(new GoodsReceiptNoteItemViewModel
                {
                    Id = item.Id,
                    GoodsReceiptNoteId = item.GoodsReceiptNoteId,
                    StockItemId = item.StockItemId,
                    StockItem = item.StockItem.Name,
                    UnitOfMeasureId = item.StockItem.UnitOfMeasureId,
                    UnitOfMeasure = item.StockItem.UnitOfMeasure.Name,
                    LocationId = item.LocationId,
                    Location = item.Location.Name,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Amount = item.Amount,
                    Note = item.Note,
                });
            });
            #endregion

            return Json(new { data = result });
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult New() => View(nameof(Detail));

        [HttpGet]
        public IActionResult Detail(string id = "") => View();

        [HttpPost]
        public ActionResult Save(GoodsReceiptNote model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    lock (_savelock)
                    {
                        if (model.Id == Guid.Empty)
                        {
                            model.Id = Guid.NewGuid();
                            model.No = GetNoVoucher(model.Id);
                            model.SequenceNo = _context.GoodsReceiptNotes.AsNoTracking().OrderByDescending(d => d.SequenceNo).FirstOrDefault()?.SequenceNo + 1 ?? 1;
                            _context.GoodsReceiptNotes.Add(model);
                        }
                        else
                        {
                            var existsEntity = _context.GoodsReceiptNotes.AsNoTracking().First(v => v.Id == model.Id);

                            if (existsEntity == null)
                            {
                                Response.StatusCode = 404;
                                return Json($"{model.GetType().Name} not found");
                            }

                            _context.GoodsReceiptNoteItems.RemoveRange(_context.GoodsReceiptNoteItems.Where(i => i.GoodsReceiptNoteId == model.Id));
                            _context.GoodsReceiptNoteItems.AddRange(model.Items);
                            _context.GoodsReceiptNotes.Update(model);
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
            var model = _context.GoodsReceiptNotes.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.GoodsReceiptNoteItems.RemoveRange(model.Items);
                _context.GoodsReceiptNotes.Remove(model);

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

            var entity = _context.GoodsReceiptNotes.AsNoTracking().Where(v => v.Id != id);

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
