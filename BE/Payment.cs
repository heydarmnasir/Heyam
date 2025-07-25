using System;
using System.Collections.Generic;

namespace BE
{
    public class Payment
    {
        public int Id { get; set; }
        public int Service { get; set; }
        public int PaymentStatus { get; set; }
        public decimal? Amount { get; set; }
        public bool IsAmountPaid { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public bool IsReminderSet { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsReminderDone { get; set; }
        public string Description { get; set; }     
        public bool DeleteStatus { get; set; }

        // Navigation properties
        public int UserRole { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }     
        public int? CaseId { get; set; }
        public virtual Case Case { get; set; }
        public virtual ICollection<PaymentAttachment> PaymentAttachments { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }


        #region ConstructorMethod
        public Payment()
        {
            DeleteStatus = false;
        }
        #endregion
    }
}
