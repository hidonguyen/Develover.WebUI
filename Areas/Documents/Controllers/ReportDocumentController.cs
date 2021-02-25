using Develover.WebUI.Areas.Documents.Models;
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

namespace Develover.WebUI.Areas.Documents.Controllers
{
    [Area("Documents")]
    [Route("documents/report/{action=index}")]
    public class ReportDocumentController : Controller
    {
        private readonly ILogger<ReportDocumentController> _logger;
        private readonly DataContext _context;

        public ReportDocumentController(ILogger<ReportDocumentController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region Inbox List
        public IActionResult InboxList()
        {
            ReportDocumentViewModel model = new ReportDocumentViewModel
            {
                DataUrl = "/documents/report/GetInboxList?filterFromDate={filterFromDate}" +
                "&fromDate={fromDate}" +
                "&filterUntilDate={filterUntilDate}" +
                "&untilDate={untilDate}" +
                "&documentTypeId={documentTypeId}" +
                "&publishedPlaceId={publishedPlaceId}" +
                "&receiverId={receiverId}" +
                "&signerId={signerId}" +
                "&approverId={approverId}",
                Columns = new List<ColumnBootstrapTable>
                {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "issueDate", title = "Issue Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "documentTypeId", visible = false },
                    new ColumnBootstrapTable { field = "documentType", title = "Type" },
                    new ColumnBootstrapTable { field = "documentNo", title = "Document No", formatter = "inboxDocumentDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "publishedDate", title = "Published Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "publishedPlaceId", visible = false },
                    new ColumnBootstrapTable { field = "publishedPlace", title = "Published Place" },
                    new ColumnBootstrapTable { field = "title", title = "Title" },
                    new ColumnBootstrapTable { field = "description", title = "Description" },
                    new ColumnBootstrapTable { field = "totalPages", title = "Total Pages", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "receiverId", visible = false },
                    new ColumnBootstrapTable { field = "receiver", title = "Receiver" },
                    new ColumnBootstrapTable { field = "signerId", visible = false },
                    new ColumnBootstrapTable { field = "signer", title = "Signer" },
                    new ColumnBootstrapTable { field = "approverId", visible = false },
                    new ColumnBootstrapTable { field = "approver", title = "Approver" }
                }
            };


            ViewData["Types"] = _context.DocumentTypes.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["PublishedPlaces"] = _context.PublishedPlaces.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();

            return View(model);
        }
        public IActionResult GetInboxList(bool filterFromDate, DateTime fromDate, bool filterUntilDate, DateTime untilDate,
           string documentTypeId, string publishedPlaceId, string receiverId, string signerId, string approverId)
        {
            documentTypeId = documentTypeId != "null" ? documentTypeId : "00000000-0000-0000-0000-000000000000";
            publishedPlaceId = publishedPlaceId != "null" ? publishedPlaceId : "00000000-0000-0000-0000-000000000000";
            receiverId = receiverId != "null" ? receiverId : "00000000-0000-0000-0000-000000000000";
            signerId = signerId != "null" ? signerId : "00000000-0000-0000-0000-000000000000";
            approverId = approverId != "null" ? approverId : "00000000-0000-0000-0000-000000000000";

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
                            ParameterName = "documentTypeId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =documentTypeId
                        },
                        new SqlParameter() {
                            ParameterName = "publishedPlaceId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =publishedPlaceId
                        },
                        new SqlParameter() {
                            ParameterName = "receiverId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =receiverId
                        },
                        new SqlParameter() {
                            ParameterName = "signerId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =signerId
                        },
                        new SqlParameter() {
                            ParameterName = "approverId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =approverId
                        }
                };
            var dataset = new DataSet().ExecuteStoredProcedure(_context, "SP_InboxList", param);
            return Json(new { rows = dataset.ToListDictionary() });
        }
        #endregion

        #region Outbox List
        public IActionResult OutboxList()
        {
            ReportDocumentViewModel model = new ReportDocumentViewModel
            {
                DataUrl = "/documents/report/GetOutboxList?filterFromDate={filterFromDate}" +
                "&fromDate={fromDate}" +
                "&filterUntilDate={filterUntilDate}" +
                "&untilDate={untilDate}" +
                "&documentTypeId={documentTypeId}" +
                "&publishedPlaceId={publishedPlaceId}" +
                "&deliverId={deliverId}" +
                "&signerId={signerId}" +
                "&approverId={approverId}",
                Columns = new List<ColumnBootstrapTable>
                {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "issueDate", title = "Issue Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "documentTypeId", visible = false },
                    new ColumnBootstrapTable { field = "documentType", title = "Type" },
                    new ColumnBootstrapTable { field = "documentNo", title = "Document No", formatter = "outboxDocumentDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "publishedDate", title = "Published Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "publishedPlaceId", visible = false },
                    new ColumnBootstrapTable { field = "publishedPlace", title = "Published Place" },
                    new ColumnBootstrapTable { field = "destination", title = "Destination" },
                    new ColumnBootstrapTable { field = "title", title = "Title" },
                    new ColumnBootstrapTable { field = "description", title = "Description" },
                    new ColumnBootstrapTable { field = "totalCopies", title = "Total Copies", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "totalPages", title = "Total Pages", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "deliverId", visible = false },
                    new ColumnBootstrapTable { field = "deliver", title = "Deliver" },
                    new ColumnBootstrapTable { field = "signerId", visible = false },
                    new ColumnBootstrapTable { field = "signer", title = "Signer" },
                    new ColumnBootstrapTable { field = "approverId", visible = false },
                    new ColumnBootstrapTable { field = "approver", title = "Approver" }
                }
            };

            ViewData["Types"] = _context.DocumentTypes.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["PublishedPlaces"] = _context.PublishedPlaces.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();

            return View(model);
        }
        public IActionResult GetOutboxList(bool filterFromDate, DateTime fromDate, bool filterUntilDate, DateTime untilDate,
           string documentTypeId, string publishedPlaceId, string deliverId, string signerId, string approverId)
        {
            documentTypeId = documentTypeId != "null" ? documentTypeId : "00000000-0000-0000-0000-000000000000";
            publishedPlaceId = publishedPlaceId != "null" ? publishedPlaceId : "00000000-0000-0000-0000-000000000000";
            deliverId = deliverId != "null" ? deliverId : "00000000-0000-0000-0000-000000000000";
            signerId = signerId != "null" ? signerId : "00000000-0000-0000-0000-000000000000";
            approverId = approverId != "null" ? approverId : "00000000-0000-0000-0000-000000000000";

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
                            ParameterName = "documentTypeId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =documentTypeId
                        },
                        new SqlParameter() {
                            ParameterName = "publishedPlaceId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =publishedPlaceId
                        },
                        new SqlParameter() {
                            ParameterName = "deliverId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =deliverId
                        },
                        new SqlParameter() {
                            ParameterName = "signerId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =signerId
                        },
                        new SqlParameter() {
                            ParameterName = "approverId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =approverId
                        }
                };
            var dataset = new DataSet().ExecuteStoredProcedure(_context, "SP_OutboxlList", param);
            return Json(new { rows = dataset.ToListDictionary() });
        }
        #endregion

        #region Internal List
        public IActionResult InternalList()
        {
            ReportDocumentViewModel model = new ReportDocumentViewModel
            {
                DataUrl = "/documents/report/GetInternalList?filterFromDate={filterFromDate}" +
                "&fromDate={fromDate}" +
                "&filterUntilDate={filterUntilDate}" +
                "&untilDate={untilDate}" +
                "&documentTypeId={documentTypeId}" +
                "&fromDepartmentId={fromDepartmentId}" +
                "&toDepartmentId={toDepartmentId}" +
                "&receiverId={receiverId}" +
                "&signerId={signerId}" +
                "&approverId={approverId}",
                Columns = new List<ColumnBootstrapTable>
                {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "issueDate", title = "Issue Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "documentTypeId", visible = false },
                    new ColumnBootstrapTable { field = "documentType", title = "Type" },
                    new ColumnBootstrapTable { field = "documentNo", title = "Document No", formatter = "internalDocumentDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "publishedDate", title = "Published Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "fromDepartmentId", visible = false },
                    new ColumnBootstrapTable { field = "fromDepartment", title = "From Department" },
                    new ColumnBootstrapTable { field = "toDepartmentId", visible = false },
                    new ColumnBootstrapTable { field = "toDepartment", title = "To Department" },
                    new ColumnBootstrapTable { field = "title", title = "Title" },
                    new ColumnBootstrapTable { field = "description", title = "Description" },
                    new ColumnBootstrapTable { field = "totalCopies", title = "Total Copies", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "totalPages", title = "Total Pages", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "receiverId", visible = false },
                    new ColumnBootstrapTable { field = "receiver", title = "Receiver" },
                    new ColumnBootstrapTable { field = "signerId", visible = false },
                    new ColumnBootstrapTable { field = "signer", title = "Signer" },
                    new ColumnBootstrapTable { field = "approverId", visible = false },
                    new ColumnBootstrapTable { field = "approver", title = "Approver" }
                }
            };
            ViewData["Types"] = _context.DocumentTypes.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "All" }).ToList();

            return View(model);
        }
        public IActionResult GetInternalList(bool filterFromDate, DateTime fromDate, bool filterUntilDate, DateTime untilDate,
           string documentTypeId, string fromDepartmentId, string toDepartmentId, string receiverId, string signerId, string approverId)
        {
            documentTypeId = documentTypeId != "null" ? documentTypeId : "00000000-0000-0000-0000-000000000000";
            fromDepartmentId = fromDepartmentId != "null" ? fromDepartmentId : "00000000-0000-0000-0000-000000000000";
            toDepartmentId = toDepartmentId != "null" ? toDepartmentId : "00000000-0000-0000-0000-000000000000";
            receiverId = receiverId != "null" ? receiverId : "00000000-0000-0000-0000-000000000000";
            signerId = signerId != "null" ? signerId : "00000000-0000-0000-0000-000000000000";
            approverId = approverId != "null" ? approverId : "00000000-0000-0000-0000-000000000000";

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
                            ParameterName = "documentTypeId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =documentTypeId
                        },
                        new SqlParameter() {
                            ParameterName = "fromDepartmentId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =fromDepartmentId
                        },
                        new SqlParameter() {
                            ParameterName = "toDepartmentId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =toDepartmentId
                        },
                        new SqlParameter() {
                            ParameterName = "receiverId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =receiverId
                        },
                        new SqlParameter() {
                            ParameterName = "signerId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =signerId
                        },
                        new SqlParameter() {
                            ParameterName = "approverId",
                            SqlDbType =  SqlDbType.VarChar,
                            Direction = ParameterDirection.Input,
                            Value =approverId
                        }
                };

            var dataset = new DataSet().ExecuteStoredProcedure(_context, "SP_InternalList", param);
            return Json(new { rows = dataset.ToListDictionary() });
        }
        #endregion
    }
}
