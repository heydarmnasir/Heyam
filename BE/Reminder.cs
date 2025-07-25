using System;
using System.Globalization;

namespace BE
{
    public class Reminder
    {
        public int Id { get; set; }
        public string Title { get; set; }               // عنوان یادآور
        public string Description { get; set; }          // توضیحات
        public DateTime ReminderDate { get; set; }       // تاریخ یادآوری
        public int UserRole { get; set; }
        public int UserId { get; set; }                  // کدام کاربر ساخته؟
        public virtual User User { get; set; }
        public int? InteractionId { get; set; }          // اگر مربوط به تعامل است
        public virtual Interaction Interaction { get; set; }
        public int? CorrespondenceId { get; set; }       // اگر مربوط به مکاتبه است
        public virtual Correspondence Correspondence { get; set; }
        public int? LegalBriefId { get; set; }              // اگر مربوط به لایحه است
        public virtual LegalBrief LegalBrief { get; set; }
        public int? PaymentId { get; set; }              // اگر مربوط به پرداخت است
        public virtual Payment Payment { get; set; }
        public int? MeetingId { get; set; }              // اگر مربوط به جلسه است
        public virtual Meeting Meeting { get; set; }
        public int? PersonalNotesId { get; set; }              // اگر مربوط به یادداشت شخصی است
        public virtual PersonalNote PersonalNote { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserTypeText
        {
            get
            {
                if (UserRole == 0) return "وکیل";
                if (UserRole == 1) return "منشی";
                return "نامشخص";
            }
        }
        public string SetDateShamsi
        {
            get
            {
                PersianCalendar pc = new PersianCalendar();
                return $"{pc.GetYear(ReminderDate):0000}/{pc.GetMonth(ReminderDate):00}/{pc.GetDayOfMonth(ReminderDate):00}";
            }
        }
    }
}