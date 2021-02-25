using Develover.WebUI.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Areas.Documents.Models
{
    public class AppendixViewModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime IssueDate { get; set; }
        public string AppendixNo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AttachmentId { get; set; }

    }
}
