﻿using Develover.WebUI.Areas.Documents.Models;
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

namespace Develover.WebUI.Areas.Documents.Controllers
{
    [Area("Documents")]
    [Route("documents/inbox/{action=index}")]
    public class InboxDocumentController : Controller
    {
        private readonly object _savelock = new object();
        private readonly ILogger<InboxDocumentController> _logger;
        private readonly DataContext _context;

        private static List<ColumnBootstrapTable> DetailColumns => new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "ownerId", visible = false },
                    new ColumnBootstrapTable { field = "sequenceNo", title = "#", sortable = false, width = "20" },
                    new ColumnBootstrapTable { field = "name", title = "Name" },
                    new ColumnBootstrapTable { field = "extension", title = "Extension", width = "150" },
                    new ColumnBootstrapTable { field = "note", title = "Note"},
                    new ColumnBootstrapTable { field = "attachment", width = "50", formatter = "attachmentFormatter", events = "window.attachmentEvents", clickToSelect = false, sortable = false },
                    new ColumnBootstrapTable { field = "operate", width = "50", formatter = "operateFormatter", events = "window.operateEvents", clickToSelect = false, sortable = false }
                };
        private static List<ColumnBootstrapTable> DetailAppendixColumns => new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "ownerId", visible = false },
                    new ColumnBootstrapTable { field = "sequenceNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter"},
                    new ColumnBootstrapTable { field = "issueDate", title = "Issue Date",formatter="dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "appendixNo", title = "Appendix No" },
                    new ColumnBootstrapTable { field = "title", title = "Title", width = "150" },
                    new ColumnBootstrapTable { field = "description", title = "Description"},
                    new ColumnBootstrapTable { field = "attachmentId", visible = false},
                    new ColumnBootstrapTable { field = "attachment", width = "25", formatter = "attachmentAppendixViewFormatter", events = "window.attachmentAppendixEvents", clickToSelect = false, sortable = false },
                    new ColumnBootstrapTable { field = "attachment", width = "75", formatter = "attachmentAppendixEditFormatter", events = "window.attachmentAppendixEvents", clickToSelect = false, sortable = false },
                    new ColumnBootstrapTable { field = "operate", width = "50", formatter = "operateFormatter", events = "window.operateAppendixEvents", clickToSelect = false, sortable = false }
                };

        public InboxDocumentController(ILogger<InboxDocumentController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        private void SetSelectPickerSource()
        {
            ViewData["Types"] = _context.DocumentTypes.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FullName }).ToList();
            ViewData["PublishedPlaces"] = _context.PublishedPlaces.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["DocumentStatus"] = _context.DocumentStatus.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
        }

        public IActionResult Index()
        {
            var model = new VoucherViewModel
            {
                DataUrl = "/documents/inbox/getdocuments",
                Columns = new List<ColumnBootstrapTable> {
                    new ColumnBootstrapTable { field = "seqNo", title = "#", sortable = false, width = "20", formatter = "seqNoFormatter" },
                    new ColumnBootstrapTable { field = "id", visible = false },
                    new ColumnBootstrapTable { field = "issueDate", title = "Issue Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "documentTypeId", visible = false },
                    new ColumnBootstrapTable { field = "documentType", title = "Type" },
                    new ColumnBootstrapTable { field = "refNo", title = "Ref No" },
                    new ColumnBootstrapTable { field = "documentNo", title = "Document No", formatter = "inboxDocumentDrillDownFormatter" },
                    new ColumnBootstrapTable { field = "publishedDate", title = "Published Date", formatter = "dateFormatter", sorter = "dateSorter" },
                    new ColumnBootstrapTable { field = "publishedPlaceId", visible = false },
                    new ColumnBootstrapTable { field = "publishedPlace", title = "Published Place" },
                    new ColumnBootstrapTable { field = "title", title = "Title" },
                    new ColumnBootstrapTable { field = "description", title = "Description" },
                    new ColumnBootstrapTable { field = "totalPages", title = "Total Pages", formatter = "numberFormatterQuantity", footerFormatter = "summaryFooterFormatterQuantity" },
                    new ColumnBootstrapTable { field = "departmentId", visible = false },
                    new ColumnBootstrapTable { field = "department", title = "Department" },
                    new ColumnBootstrapTable { field = "receiverId", visible = false },
                    new ColumnBootstrapTable { field = "receiver", title = "Receiver" },
                    new ColumnBootstrapTable { field = "approverId", visible = false },
                    new ColumnBootstrapTable { field = "approver", title = "Approver" },
                    new ColumnBootstrapTable { field = "signerId", visible = false },
                    new ColumnBootstrapTable { field = "signer", title = "Signer" },
                    new ColumnBootstrapTable { field = "documentStatusId", visible = false },
                    new ColumnBootstrapTable { field = "documentStatus", title = "Status" }
                }
            };
            return View(model);
        }

        public async Task<IActionResult> GetDocuments()
        {
            List<InboxDocumentViewModel> result = new List<InboxDocumentViewModel>();

            var inboxes = _context.InboxDocuments
                .Include(d => d.DocumentType)
                .Include(d => d.Department)
                .Include(d => d.Receiver)
                .Include(d => d.Approver)
                .Include(d => d.Signer)
                .Include(d => d.PublishedPlace)
                .Include(d => d.DocumentStatus)
                .AsQueryable();

            #region Map entity to ViewModel
            await inboxes.ForEachAsync(inbox =>
            {
                result.Add(new InboxDocumentViewModel
                {
                    Id = inbox.Id,
                    DocumentTypeId = inbox.DocumentTypeId,
                    DocumentType = inbox.DocumentType.Name,
                    IssueDate = inbox.IssueDate,
                    RefNo = inbox.RefNo,
                    DepartmentId = inbox.DepartmentId,
                    Department = inbox.Department.Name,
                    ReceiverId = inbox.ReceiverId,
                    Receiver = inbox.Receiver.FullName,
                    ApproverId = inbox.ApproverId,
                    Approver = inbox.Approver.FullName,
                    SignerId = inbox.SignerId,
                    Signer = inbox.Signer.FullName,

                    DocumentNo = inbox.DocumentNo,
                    PublishedDate = inbox.PublishedDate,
                    PublishedPlaceId = inbox.PublishedPlaceId,
                    PublishedPlace = inbox.PublishedPlace.Name,
                    Title = inbox.Title,
                    Description = inbox.Description,
                    TotalPages = inbox.TotalPages,

                    DocumentStatusId = inbox.DocumentStatusId,
                    DocumentStatus = inbox.DocumentStatus?.Name
                });
            });
            #endregion

            return Json(new { rows = result });
        }

        public async Task<IActionResult> GetAppendices(string id)
        {
            List<AppendixViewModel> result = new List<AppendixViewModel>();
            _ = Guid.TryParse(id, out Guid guid);
            var appendices = _context.Appendices
                .Where(a => a.OwnerId == guid)
                .OrderBy(a => a.IssueDate)
                .AsQueryable();

            #region Map entity to ViewModel
            await appendices.ForEachAsync(appendix =>
            {
                result.Add(new AppendixViewModel
                {
                    Id = appendix.Id,
                    OwnerId = appendix.OwnerId,
                    IssueDate = appendix.IssueDate,
                    AppendixNo = appendix.AppendixNo,
                    Title = appendix.Title,
                    Description = appendix.Description,
                    AttachmentId = appendix.AttachmentId,
                });
            });
            #endregion

            return Json(new { rows = result });
        }
        [HttpGet]
        public IActionResult New()
        {
            var model = new InboxDocument
            {
                Id = Guid.NewGuid(),
                IssueDate = DateTime.Now,
                PublishedDate = DateTime.Now,
                TotalPages = 0,
                Attachments = new List<Attachment>(),
                Appendices = new List<Appendix>(),
            };

            model.DetailDataUrl = $"/attachment/getattachments?id={model.Id}";
            model.DetailColumns = DetailColumns;

            model.DetailAppendixDataUrl = $"/documents/inbox/getappendices?id={model.Id}";
            model.DetailAppendixColumns = DetailAppendixColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.InboxDocuments.FirstOrDefault(d => d.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.Attachments = _context.Attachments.Where(a => a.OwnerId == model.Id).ToList();
            model.Appendices = _context.Appendices.Where(a => a.OwnerId == model.Id).ToList();

            model.AllAttachmentIds.AddRange(model.Attachments.Select(x => x.Id));
            model.AllAttachmentIds.AddRange(model.Appendices.Select(x => x.AttachmentId));

            model.CurrentAttachmentIds.AddRange(model.Attachments.Select(x => x.Id));
            model.CurrentAttachmentIds.AddRange(model.Appendices.Select(x => x.AttachmentId));

            model.Attachments.ForEach(a => a.FileContent = null);

            model.DetailDataUrl = $"/attachment/getattachments?id={modelId}";
            model.DetailColumns = DetailColumns;

            model.DetailAppendixDataUrl = $"/documents/inbox/getappendices?id={model.Id}";
            model.DetailAppendixColumns = DetailAppendixColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            _ = Guid.TryParse(id, out Guid modelId);

            var model = _context.InboxDocuments
                .Include(d => d.CreatedByUser).Include(d => d.UpdatedByUser)
                .FirstOrDefault(d => d.Id == modelId);

            if (model == null)
            {
                return View("_404");
            }

            model.DetailDataUrl = $"/attachment/getattachments?id={modelId}";
            model.DetailColumns = DetailColumns;

            model.DetailAppendixDataUrl = $"/documents/inbox/getappendices?id={model.Id}";
            model.DetailAppendixColumns = DetailAppendixColumns;

            SetSelectPickerSource();

            return View(model);
        }

        [HttpPost]
        public IActionResult ValidateAppendixModel(Appendix model)
        {
            if (ModelState.IsValid)
                return Ok(new AppendixViewModel
                {
                    Id = model.Id,
                    OwnerId = model.OwnerId,
                    IssueDate = model.IssueDate,
                    AppendixNo = model.AppendixNo,
                    Title = model.Title,
                    Description = model.Description
                });

            Response.StatusCode = 400;
            return Json(ModelState.AllModels());
        }

        [HttpPost]
        public ActionResult Save(int mode, InboxDocument model)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    lock (_savelock)
                    {
                        model.DocumentType = null;
                        model.Department = null;
                        model.Receiver = null;
                        model.Approver = null;
                        model.Signer = null;
                        model.PublishedPlace = null;
                        model.DocumentStatus = null;


                        if (model.Attachments == null) model.Attachments = new List<Attachment>();
                        if (model.Appendices == null) model.Appendices = new List<Appendix>();

                        if (mode == 1)
                        {
                            model.RefNo = GetNoVoucher(model.Id);
                            model.SequenceNo = _context.InboxDocuments.OrderByDescending(d => d.SequenceNo).FirstOrDefault()?.SequenceNo + 1 ?? 1;
                            _context.InboxDocuments.Add(model);
                            _context.Appendices.AddRange(model.Appendices);
                        }
                        else
                        {
                            var existsEntity = _context.InboxDocuments.AsNoTracking().FirstOrDefault(d => d.Id == model.Id);

                            if (existsEntity == null)
                            {
                                Response.StatusCode = 404;
                                return Json($"{model.GetType().Name} not found");
                            }

                            _context.InboxDocuments.Update(model);
                            _context.Appendices.RemoveRange(_context.Appendices.Where(d => d.OwnerId == existsEntity.Id));
                            _context.Appendices.AddRange(model.Appendices);
                        }
                        _context.Attachments.RemoveRange(_context.Attachments.Where(x => model.AllAttachmentIds.Contains(x.Id) && !model.CurrentAttachmentIds.Contains(x.Id)));

                        _context.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.RollbackAsync();
                    Response.StatusCode = 500;
                    return Json(ex.Message);
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
            var model = _context.InboxDocuments.FirstOrDefault(d => d.Id == modelId);

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"{model.GetType().Name} not found");
            }

            model.Attachments = _context.Attachments.Where(x => x.OwnerId == model.Id).ToList();
            model.Appendices = _context.Appendices.Where(x => x.OwnerId == model.Id).ToList();

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Attachments.RemoveRange(_context.Attachments.Where(x => x.OwnerId == model.Id));
                _context.Attachments.RemoveRange(_context.Attachments.Where(x => model.Appendices.Select(a => a.Id).Contains(x.OwnerId)));
                _context.Appendices.RemoveRange(_context.Appendices.Where(x => x.OwnerId == model.Id));
                _context.InboxDocuments.Remove(model);

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

        [HttpPost]
        public ActionResult Cancel(int mode, InboxDocument model)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (mode == 1)
                {
                    _context.Attachments.RemoveRange(_context.Attachments.Where(a => model.AllAttachmentIds.Contains(a.Id)));
                }
                else
                {

                    _context.Attachments.RemoveRange(_context.Attachments.Where(a => model.AllAttachmentIds.Contains(a.Id) && !model.CurrentAttachmentIds.Contains(a.Id)));
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

            var entity = _context.InboxDocuments.AsNoTracking().Where(v => v.Id != id);

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

            var sttNo = entity.OrderByDescending(v => v.SequenceNo).Select(v => v.RefNo).FirstOrDefault();

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
