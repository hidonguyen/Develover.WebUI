using Develover.WebUI.Areas.Stationery.Models;
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

namespace Develover.WebUI.Areas.Stationery.Controllers
{
    [Area("Stationery")]
    [Route("stationery/OpeningStock/{action=index}")]
    public class OpeningStockController : Controller
    {
        private readonly object _savelock = new object();
        private readonly ILogger<OpeningStockController> _logger;
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
                    new ColumnBootstrapTable { field = "amount", title = "Amount", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "note", title = "Note"},
                    new ColumnBootstrapTable { field = "operate", width = "50", formatter = "operateFormatter", events = "window.operateEvents", clickToSelect = false, sortable = false }
                };

        public OpeningStockController(ILogger<OpeningStockController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        private void SetSelectPickerSource()
        {
            ViewData["StockItems"] = _context.StockItems.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["UnitOfMeasures"] = _context.UnitOfMeasures.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Locations"] = _context.Locations.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
        }

        public IActionResult Index()
        {
            var model = new VoucherViewModel
            {
                DataUrl = "/stationery/openingStock/GetOpeningStocks",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "dateUpdate", title = "Date Update", formatter = "openingStockDrillDownFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "amount", title = "Amount", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "note", title = "Note" }
                }
            };
            return View(model);
        }

        public async Task<IActionResult> GetOpeningStocks()
        {
            List<OpeningStockViewModel> result = new List<OpeningStockViewModel>();

            var entitys = _context.OpeningStocks.Include(x=>x.Items)
                .AsQueryable();

            #region Map entity to ViewModel
            await entitys.ForEachAsync(entity =>
            {
                result.Add(new OpeningStockViewModel
                {
                    Id = entity.Id,
                    DateUpdate = entity.DateUpdate,
                    Quantity = entity.Quantity,
                    Amount = entity.Amount,
                    Note = entity.Note,
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        public async Task<IActionResult> GetOpeningStockItems(string id)
        {
            List<OpeningStockItemViewModel> result = new List<OpeningStockItemViewModel>();

            _ = Guid.TryParse(id, out Guid modelId);

            var items = _context.OpeningStockItems
                .Include(i => i.StockItem)
                    .ThenInclude(i => i.UnitOfMeasure)
                .Include(i => i.Location)
                .OrderBy(i => i.SequenceNo)
                .Where(i => i.OpeningStockId == modelId)
                .AsQueryable();

            #region Map entity to ViewModel
            await items.ForEachAsync(item =>
            {
                result.Add(new OpeningStockItemViewModel
                {
                    Id = item.Id,
                    OpeningStockId = item.OpeningStockId,
                    StockItemId = item.StockItemId,
                    StockItem = item.StockItem.Name,
                    UnitOfMeasureId = item.StockItem.UnitOfMeasureId,
                    UnitOfMeasure = item.StockItem.UnitOfMeasure.Name,
                    LocationId = item.LocationId,
                    Location = item.Location.Name,
                    Quantity = item.Quantity,
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
            var entity = _context.OpeningStocks.Where(o => o.DateUpdate.Year.Equals(DateTime.Now.Year)).FirstOrDefault();
            SetSelectPickerSource();
            if (entity == null)
            {
                entity = new OpeningStock
                {
                    Id = Guid.NewGuid(),
                    DateUpdate = new DateTime(DateTime.Now.Year, 1, 1),
                    Items = new List<OpeningStockItem>(),
                    DetailDataUrl = $"/stationery/openingstock/getopeningstockitems?id={Guid.Empty}",
                    DetailColumns = DetailColumns
                };


                return View(entity);
            }
            else
            {
                entity.DetailDataUrl = $"/stationery/openingstock/getopeningstockitems?id={entity.Id}";
                entity.DetailColumns = DetailColumns;
                return View("Edit", entity);

            }
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.OpeningStocks.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/stationery/openingstock/getopeningstockitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.OpeningStocks.Include(u => u.CreatedByUser).Include(u => u.UpdatedByUser).Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/stationery/openingstock/getopeningstockitems?id={modelId}";
            model.DetailColumns = DetailColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(int mode, OpeningStock model)
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
                            _context.OpeningStocks.Add(model);
                        }
                        else
                        {
                            var existsEntity = _context.OpeningStocks.AsNoTracking().First(v => v.Id == model.Id);

                            if (existsEntity == null)
                            {
                                Response.StatusCode = 404;
                                return Json($"{model.GetType().Name} not found");
                            }

                            _context.OpeningStockItems.RemoveRange(_context.OpeningStockItems.Where(i => i.OpeningStockId == model.Id));
                            _context.OpeningStockItems.AddRange(model.Items);
                            _context.OpeningStocks.Update(model);
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
            var model = _context.OpeningStocks.Include(v => v.Items).FirstOrDefault(v => v.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.OpeningStockItems.RemoveRange(model.Items);
                _context.OpeningStocks.Remove(model);

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
    }
}
