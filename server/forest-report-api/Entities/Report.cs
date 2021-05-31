using System;
using forest_report_api.Entities.Enums;

namespace forest_report_api.Entities
{
    public class Report : BaseEntity
    {
        public string FormCollectionId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? ReplyDate { get; set; }
        public StatusReport? StatusReport { get; set; }
        public AdminStatusReport? AdminStatusReport { get; set; }
        public string ReportTypeId { get; set; }
        public ReportType ReportType { get; set; }
        public string Description { get; set; }
        public string UserCheckinIntervalId { get; set; }
        public UserCheckinInterval UserCheckinInterval { get; set; }
        public string AttachmentFileId { get; set; }
        public AttachmentFile AttachmentFile { get; set; }
        public string Note { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; }
        public bool IsRead { get; set; }
    }
}