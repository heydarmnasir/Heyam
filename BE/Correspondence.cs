using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE
{
    public class Correspondence
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } // عنوان مکاتبه
        public string CorrespondenceNumber { get; set; } // شماره نامه
        public string CorrespondenceDate { get; set; } // تاریخ مکاتبه
        public string Type { get; set; } // نوع مکاتبه (قضایی، اداری، اطلاعیه و...)
        public string Sender { get; set; } // مرجع ارسال‌کننده
        public string Receiver { get; set; } // مرجع گیرنده
        public string Subject { get; set; } // موضوع مکاتبه
        public string CorrespondenceDescription { get; set; } // شرح کامل
        public int Status { get; set; } // وضعیت (پاسخ داده شده، در حال پیگیری، ...)
        public DateTime CreatedAt { get; set; }
        public bool DeleteStatus { get; set; }
        // یادآور
        public bool IsReminderSet { get; set; } // آیا یادآوری فعال است؟
        public bool IsReminderDone { get; set; } // آیا یادآوری انجام شده؟     
        public DateTime? ReminderDate { get; set; } // تاریخ یادآوری (در صورت فعال بودن)

        public int UserRole { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        // ارتباط با پرونده
        public int CaseId { get; set; }
        public virtual Case Case { get; set; }
        // پیوست‌ها
        public virtual ICollection<CorrespondenceAttachment> CorrespondenceAttachments { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }


        #region ConstructorMethod
        public Correspondence()
        {
            DeleteStatus = false;
        }
        #endregion
    }
}