using System;
using System.Collections.Generic;

namespace BE
{
    public class Interaction
    {
        public int Id { get; set; }
        public string Type { get; set; } // نوع تعامل
        public string InteractionDate { get; set; } // تاریخ تعامل
        public DateTime? ReminderDate { get; set; } // تاریخ یادآوری
        public bool IsReminderSet { get; set; } // آیا یادآوری دارد؟
        public bool IsReminderDone { get; set; } // آیا یادآوری انجام شده؟
        public string Note { get; set; } // یادداشت
        public DateTime CreatedAt { get; set; } = DateTime.Now; // تاریخ ثبت       
        public bool DeleteStatus { get; set; }


        //Associations
        public int UserRole { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }

        #region ConstructorMethod
        public Interaction()
        {
            DeleteStatus = false;
        }
        #endregion
    }    
}
