using System;

namespace BE
{
    public class CaseAttachment
    {
        public int Id { get; set; }       
        public string FileName { get; set; }
        public string FilePath { get; set; } // مسیر فیزیکی فایل روی دیسک
        public DateTime? UploadedAt { get; set; }

        // Navigation Properties
        public int CaseId { get; set; }
        public virtual Case Case { get; set; }
    }
}