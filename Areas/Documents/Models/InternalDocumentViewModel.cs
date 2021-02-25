using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Areas.Documents.Models
{
    public class InternalDocumentViewModel
    {
        public Guid Id { get; set; }
        public Guid DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public DateTime IssueDate { get; set; }
        public int SequenceNo { get; set; }
        public Guid FromDepartmentId { get; set; }
        public string FromDepartment { get; set; }
        public Guid DeliverId { get; set; }
        public string Deliver { get; set; }
        public Guid ToDepartmentId { get; set; }
        public string ToDepartment { get; set; }
        public Guid ReceiverId { get; set; }
        public string Receiver { get; set; }
        public Guid? ApproverId { get; set; }
        public string Approver { get; set; }
        public Guid? SignerId { get; set; }
        public string Signer { get; set; }

        public string DocumentNo { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double TotalCopies { get; set; }
        public double TotalPages { get; set; }

        public Guid DocumentStatusId { get; set; }
        public string DocumentStatus { get; set; }
        public DateTime DocumentStatusDate { get; set; }
        public string DocumentStatusNote { get; set; }
    }
}