using Develover.WebUI.Areas.Stationery.Models;
using Develover.WebUI.DbContexts;
using Develover.WebUI.Extensions;
using Develover.WebUI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Develover.WebUI.Areas.Stationery.Controllers
{
    [Area("Stationery")]
    [Route("Stationery/report/{action=index}")]
    public class ReportStationeryController : Controller
    {
        private readonly ILogger<ReportStationeryController> _logger;
        private readonly DataContext _context;

        public ReportStationeryController(ILogger<ReportStationeryController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region ReceiptNote List
        public IActionResult ReceiptNoteList()
        {
            ReportStationeryViewModel model = new ReportStationeryViewModel
            {
                DataUrl = "/Stationery/report/GetReceiptNoteList?filterFromDate={filterFromDate}" +
                "&fromDate={fromDate}" +
                "&filterUntilDate={filterUntilDate}" +
                "&untilDate={untilDate}" +
                "&supplierId={supplierId}" +
                "&stockItemId={stockItemId}" +
                "&locationId={locationId}",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoPidFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "pid", visible = false },
                    new ColumnBootstrapTable { field = "date", title = "Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "no", title = "No", formatter = "goodsReceiptNoteDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "supplierId", visible = false },
                    new ColumnBootstrapTable { field = "supplier", title = "Supplier" },

                    new ColumnBootstrapTable { field = "stockItemId", visible = false },
                    new ColumnBootstrapTable { field = "stockItem", title = "Item" },
                    new ColumnBootstrapTable { field = "unitOfMeasure", title = "UoM" },
                    new ColumnBootstrapTable { field = "locationId", visible = false },
                    new ColumnBootstrapTable { field = "location", title = "Location" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "price", title = "Price", formatter = "numberFormatterAmount", footerFormatter = "summaryFooterFormatterAmount" },
                    new ColumnBootstrapTable { field = "amount", title = "Amount", formatter = "numberFormatterAmountVND", footerFormatter = "summaryFooterFormatterAmountVND" },
                    new ColumnBootstrapTable { field = "note", title = "Note" },

                }
            };

            ViewData["Suppliers"] = _context.Suppliers.OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                    .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["StockItems"] = _context.StockItems.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Locations"] = _context.Locations.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();

            return View(model);
        }
        public IActionResult GetReceiptNoteList(bool filterFromDate, DateTime fromDate, bool filterUntilDate, DateTime untilDate,
           string supplierId, string stockItemId, string locationId)
        {
            supplierId = supplierId != "null" ? supplierId : "00000000-0000-0000-0000-000000000000";
            stockItemId = stockItemId != "null" ? stockItemId : "00000000-0000-0000-0000-000000000000";
            locationId = locationId != "null" ? locationId : "00000000-0000-0000-0000-000000000000";

            var param = new SqlParameter[] {
                        new SqlParameter() {
                            ParameterName = "filterFromDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterFromDate
                        }, new SqlParameter() {
                            ParameterName = "fromDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value = fromDate
                        },
                        new SqlParameter() {
                            ParameterName = "filterUntilDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterUntilDate
                        },
                        new SqlParameter() {
                            ParameterName = "untilDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value =untilDate
                        },
                        new SqlParameter() {
                            ParameterName = "supplierId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =supplierId
                        },
                        new SqlParameter() {
                            ParameterName = "stockItemId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =stockItemId
                        },
                        new SqlParameter() {
                            ParameterName = "locationId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =locationId
                        }
                };
            var dataset = new DataSet().ExecuteStoredProcedure(_context, "SP_ReceiptNoteList", param);
            return Json(new { rows = dataset.ToListDictionary() });
        }
        #endregion

        #region TotalReceiptNote List
        public IActionResult TotalReceiptNoteList()
        {
            ReportStationeryViewModel model = new ReportStationeryViewModel
            {
                DataUrl = "/Stationery/report/GetTotalReceiptNoteList?filterFromDate={filterFromDate}" +
                "&fromDate={fromDate}" +
                "&filterUntilDate={filterUntilDate}" +
                "&untilDate={untilDate}" +
                "&supplierId={supplierId}" +
                "&stockItemId={stockItemId}" +
                "&locationId={locationId}",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "stockItemId", visible = false },
                    new ColumnBootstrapTable { field = "stockItem", title = "Item" },
                    new ColumnBootstrapTable { field = "unitOfMeasure", title = "UoM" },
                    new ColumnBootstrapTable { field = "locationId", visible = false },
                    new ColumnBootstrapTable { field = "location", title = "Location" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "aVGprice", title = "AVG Price", formatter = "numberFormatterAmount", footerFormatter = "summaryFooterFormatterAmount" },
                    new ColumnBootstrapTable { field = "amount", title = "Amount", formatter = "numberFormatterAmountVND", footerFormatter = "summaryFooterFormatterAmountVND" },

                }
            };

            ViewData["Suppliers"] = _context.Suppliers.OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                    .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["StockItems"] = _context.StockItems.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Locations"] = _context.Locations.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();

            return View(model);
        }
        public IActionResult GetTotalReceiptNoteList(bool filterFromDate, DateTime fromDate, bool filterUntilDate, DateTime untilDate,
           string supplierId, string stockItemId, string locationId)
        {
            supplierId = supplierId != "null" ? supplierId : "00000000-0000-0000-0000-000000000000";
            stockItemId = stockItemId != "null" ? stockItemId : "00000000-0000-0000-0000-000000000000";
            locationId = locationId != "null" ? locationId : "00000000-0000-0000-0000-000000000000";

            var param = new SqlParameter[] {
                        new SqlParameter() {
                            ParameterName = "filterFromDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterFromDate
                        }, new SqlParameter() {
                            ParameterName = "fromDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value = fromDate
                        },
                        new SqlParameter() {
                            ParameterName = "filterUntilDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterUntilDate
                        },
                        new SqlParameter() {
                            ParameterName = "untilDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value =untilDate
                        },
                        new SqlParameter() {
                            ParameterName = "supplierId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =supplierId
                        },
                        new SqlParameter() {
                            ParameterName = "stockItemId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =stockItemId
                        },
                        new SqlParameter() {
                            ParameterName = "locationId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =locationId
                        }
                };
            var dataset = new DataSet().ExecuteStoredProcedure(_context, "SP_TotalReceiptNoteList", param);
            return Json(new { rows = dataset.ToListDictionary() });
        }
        #endregion

        #region IssueNote List
        public IActionResult IssueNoteList()
        {
            ReportStationeryViewModel model = new ReportStationeryViewModel
            {
                DataUrl = "/Stationery/report/GetIssueNoteList?filterFromDate={filterFromDate}" +
                "&fromDate={fromDate}" +
                "&filterUntilDate={filterUntilDate}" +
                "&untilDate={untilDate}" +
                "&departmentId={departmentId}" +
                "&receiverId={receiverId}" +
                "&stockItemId={stockItemId}" +
                "&locationId={locationId}",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoPidFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "pid", visible = false },
                    new ColumnBootstrapTable { field = "date", title = "Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "no", title = "No", formatter = "goodsReceiptNoteDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "departmentId", visible = false },
                    new ColumnBootstrapTable { field = "department", title = "Department" },
                    new ColumnBootstrapTable { field = "receiverId", visible = false },
                    new ColumnBootstrapTable { field = "receiver", title = "Item" },

                    new ColumnBootstrapTable { field = "stockItemId", visible = false },
                    new ColumnBootstrapTable { field = "stockItem", title = "Item" },
                    new ColumnBootstrapTable { field = "unitOfMeasure", title = "UoM" },
                    new ColumnBootstrapTable { field = "locationId", visible = false },
                    new ColumnBootstrapTable { field = "location", title = "Location" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                     new ColumnBootstrapTable { field = "note", title = "Note" },

                }
            };

            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                    .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Receivers"] = _context.Employees.OrderBy(c => c.FirstName)
                    .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList()
                    .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["StockItems"] = _context.StockItems.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Locations"] = _context.Locations.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();

            return View(model);
        }
        public IActionResult GetIssueNoteList(bool filterFromDate, DateTime fromDate, bool filterUntilDate, DateTime untilDate,
           string departmentId, string receiverId, string stockItemId, string locationId)
        {
            departmentId = departmentId != "null" ? departmentId : "00000000-0000-0000-0000-000000000000";
            receiverId = receiverId != "null" ? receiverId : "00000000-0000-0000-0000-000000000000";
            stockItemId = stockItemId != "null" ? stockItemId : "00000000-0000-0000-0000-000000000000";
            locationId = locationId != "null" ? locationId : "00000000-0000-0000-0000-000000000000";

            var param = new SqlParameter[] {
                        new SqlParameter() {
                            ParameterName = "filterFromDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterFromDate
                        }, new SqlParameter() {
                            ParameterName = "fromDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value = fromDate
                        },
                        new SqlParameter() {
                            ParameterName = "filterUntilDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterUntilDate
                        },
                        new SqlParameter() {
                            ParameterName = "untilDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value =untilDate
                        },
                        new SqlParameter() {
                            ParameterName = "departmentId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =departmentId
                        },
                        new SqlParameter() {
                            ParameterName = "receiverId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =receiverId
                        },
                        new SqlParameter() {
                            ParameterName = "stockItemId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =stockItemId
                        },
                        new SqlParameter() {
                            ParameterName = "locationId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =locationId
                        }
                };
            var dataset = new DataSet().ExecuteStoredProcedure(_context, "SP_IssueNoteList", param);
            return Json(new { rows = dataset.ToListDictionary() });
        }
        #endregion

        #region Total IssueNote List
        public IActionResult TotalIssueNoteList()
        {
            ReportStationeryViewModel model = new ReportStationeryViewModel
            {
                DataUrl = "/Stationery/report/GetTotalIssueNoteList?filterFromDate={filterFromDate}" +
                "&fromDate={fromDate}" +
                "&filterUntilDate={filterUntilDate}" +
                "&untilDate={untilDate}" +
                "&departmentId={departmentId}" +
                "&receiverId={receiverId}" +
                "&stockItemId={stockItemId}" +
                "&locationId={locationId}",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "stockItemId", visible = false },
                    new ColumnBootstrapTable { field = "stockItem", title = "Item" },
                    new ColumnBootstrapTable { field = "unitOfMeasure", title = "UoM" },
                    new ColumnBootstrapTable { field = "locationId", visible = false },
                    new ColumnBootstrapTable { field = "location", title = "Location" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },

                }
            };

            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                    .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Receivers"] = _context.Employees.OrderBy(c => c.FirstName)
                    .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList()
                    .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["StockItems"] = _context.StockItems.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Locations"] = _context.Locations.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();

            return View(model);
        }
        public IActionResult GetTotalIssueNoteList(bool filterFromDate, DateTime fromDate, bool filterUntilDate, DateTime untilDate,
           string departmentId, string receiverId, string stockItemId, string locationId)
        {
            departmentId = departmentId != "null" ? departmentId : "00000000-0000-0000-0000-000000000000";
            receiverId = receiverId != "null" ? receiverId : "00000000-0000-0000-0000-000000000000";
            stockItemId = stockItemId != "null" ? stockItemId : "00000000-0000-0000-0000-000000000000";
            locationId = locationId != "null" ? locationId : "00000000-0000-0000-0000-000000000000";

            var param = new SqlParameter[] {
                        new SqlParameter() {
                            ParameterName = "filterFromDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterFromDate
                        }, new SqlParameter() {
                            ParameterName = "fromDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value = fromDate
                        },
                        new SqlParameter() {
                            ParameterName = "filterUntilDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterUntilDate
                        },
                        new SqlParameter() {
                            ParameterName = "untilDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value =untilDate
                        },
                        new SqlParameter() {
                            ParameterName = "departmentId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =departmentId
                        },
                        new SqlParameter() {
                            ParameterName = "receiverId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =receiverId
                        },
                        new SqlParameter() {
                            ParameterName = "stockItemId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =stockItemId
                        },
                        new SqlParameter() {
                            ParameterName = "locationId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =locationId
                        }
                };
            var dataset = new DataSet().ExecuteStoredProcedure(_context, "SP_TotalIssueNoteList", param);
            return Json(new { rows = dataset.ToListDictionary() });
        }
        #endregion

        #region Inventory List
        public IActionResult InventoryList()
        {
            ReportStationeryViewModel model = new ReportStationeryViewModel
            {
                DataUrl = "/Stationery/report/GetInventoryList?filterFromDate={filterFromDate}" +
                "&fromDate={fromDate}" +
                "&filterUntilDate={filterUntilDate}" +
                "&untilDate={untilDate}" +
                "&stockItemId={stockItemId}" +
                "&locationId={locationId}",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "stockItemId", visible = false },
                    new ColumnBootstrapTable { field = "stockItem", title = "Item" },
                    new ColumnBootstrapTable { field = "unitOfMeasure", title = "UoM" },
                    new ColumnBootstrapTable { field = "locationId", visible = false },
                    new ColumnBootstrapTable { field = "location", title = "Location" },
                    new ColumnBootstrapTable { field = "openingStock", title = "Opening Stock", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "input", title = "Input", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "output", title = "Output", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "closingStock", title = "closing Stock", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                }
            };

            ViewData["StockItems"] = _context.StockItems.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Locations"] = _context.Locations.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();

            return View(model);
        }
        public IActionResult GetInventoryList(bool filterFromDate, DateTime fromDate, bool filterUntilDate, DateTime untilDate,
          string stockItemId, string locationId)
        {
            stockItemId = stockItemId != "null" ? stockItemId : "00000000-0000-0000-0000-000000000000";
            locationId = locationId != "null" ? locationId : "00000000-0000-0000-0000-000000000000";

            var param = new SqlParameter[] {
                        new SqlParameter() {
                            ParameterName = "filterFromDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterFromDate
                        }, new SqlParameter() {
                            ParameterName = "fromDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value = fromDate
                        },
                        new SqlParameter() {
                            ParameterName = "filterUntilDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterUntilDate
                        },
                        new SqlParameter() {
                            ParameterName = "untilDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value =untilDate
                        },
                        new SqlParameter() {
                            ParameterName = "stockItemId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =stockItemId
                        },
                        new SqlParameter() {
                            ParameterName = "locationId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =locationId
                        }
                };
            var dataset = new DataSet().ExecuteStoredProcedure(_context, "SP_InventoryList", param);
            return Json(new { rows = dataset.ToListDictionary() });
        }
        #endregion

        #region Import And Export Synthesis List
        public IActionResult ImportAndExportSynthesisList()
        {
            ReportStationeryViewModel model = new ReportStationeryViewModel
            {
                DataUrl = "/Stationery/report/GetImportAndExportSynthesisList?&filterUntilDate={filterUntilDate}" +
                "&untilDate={untilDate}" +
                "&stockItemId={stockItemId}" +
                "&locationId={locationId}",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "stockItemId", visible = false },
                    new ColumnBootstrapTable { field = "stockItem", title = "Item" },
                    new ColumnBootstrapTable { field = "unitOfMeasure", title = "UoM" },
                    new ColumnBootstrapTable { field = "locationId", visible = false },
                    new ColumnBootstrapTable { field = "location", title = "Location" },
                    new ColumnBootstrapTable { field = "quantity", title = "Quantity", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },

                }
            };

            ViewData["StockItems"] = _context.StockItems.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Locations"] = _context.Locations.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();

            return View(model);
        }
        public IActionResult GetImportAndExportSynthesisList(bool filterUntilDate, DateTime untilDate,
          string stockItemId, string locationId)
        {
            stockItemId = stockItemId != "null" ? stockItemId : "00000000-0000-0000-0000-000000000000";
            locationId = locationId != "null" ? locationId : "00000000-0000-0000-0000-000000000000";

            var param = new SqlParameter[] {
                        new SqlParameter() {
                            ParameterName = "filterUntilDate",
                            SqlDbType =  SqlDbType.Bit,
                            Direction = ParameterDirection.Input,
                            Value = filterUntilDate
                        },
                        new SqlParameter() {
                            ParameterName = "untilDate",
                            SqlDbType =  SqlDbType.DateTime,
                            Direction = ParameterDirection.Input,
                            Value =untilDate
                        },
                        new SqlParameter() {
                            ParameterName = "stockItemId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =stockItemId
                        },
                        new SqlParameter() {
                            ParameterName = "locationId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =locationId
                        }
                };
            var dataset = new DataSet().ExecuteStoredProcedure(_context, "SP_ImportAndExportSynthesisList", param);
            return Json(new { rows = dataset.ToListDictionary() });
        }
        #endregion
    }
}
