using System;

namespace BE
{
    public class CorrespondenceAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime? UploadedAt { get; set; }

        public int CorrespondenceId { get; set; }
        public virtual Correspondence Correspondence { get; set; }
    }
}
