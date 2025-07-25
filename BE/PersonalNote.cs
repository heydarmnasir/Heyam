using System;
using System.Collections.Generic;

namespace BE
{
    public class PersonalNote
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsReminderSet { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsReminderDone { get; set; }        
        public DateTime CreatedAt { get; set; }
        public bool DeleteStatus { get; set; }

        // Navigation Properties
        public int UserRole { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }

        #region ConstructorMethod
        public PersonalNote()
        {
            DeleteStatus = false;
        }
        #endregion
    }
}
