using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE
{
    public class Meeting
    {
        public int Id { get; set; }
        public string MeetingSubject { get; set; }
        public string MeetingPlace { get; set; }
        public DateTime MeetingDateTime { get; set; }
        public bool IsReminderMeetingSet { get; set; }
        public ReminderStage Reminder_Stage { get; set; }
        public bool IsReminderMeetingDone { get; set; }
        public string Description { get; set; }
        //ثبت مهلت قانونی
        public DateTime? MeetingStartDate { get; set; }
        public DateTime? MeetingEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeleteStatus { get; set; }

        // Navigation Properties
        public int UserRole { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public int CaseId { get; set; }
        public Case Case { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }


        #region ConstructorMethod
        public Meeting()
        {
            DeleteStatus = false;
        }
        #endregion

        // Enum پیشنهادی:
        public enum ReminderStage
        {
            None = 0,
            OneDayBefore = 1,
            OneHourBefore = 2,
            Done = 3
        }
    }
}
