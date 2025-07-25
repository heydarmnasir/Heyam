using System;

namespace BE
{
    public class PaymentAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime? UploadedAt { get; set; }

        public int PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
    }
}
