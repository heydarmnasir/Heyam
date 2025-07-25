using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE
{
    public class SmsPanel
    {
        public int Id { get; set; }
        // اگر ارسال به موکل بوده:
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }      
        // متن پیامک ارسالی
        public string Message { get; set; }
        public DateTime SentAt { get; set; }    
        // شماره خط ارسال‌کننده
        public string SenderLineNumber { get; set; }
        // شناسه گروهی (برای ارسال‌های گروهی)
        public Guid? BulkGroupId { get; set; }
        // ➕ جدید:
        public bool IsSuccess { get; set; }
        public int UserRole { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }    
    }
}