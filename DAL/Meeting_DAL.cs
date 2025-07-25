using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DAL
{
    public class Meeting_DAL
    {
        DB db = new DB();
        public int Create(Meeting meeting)
        {
            db.Meetings.Add(meeting);
            db.SaveChanges();
            return meeting.Id;
        }
        public List<Meeting> GetAllMeetings()
        {
            return db.Meetings.Include("User").Include("Client").Include("Case").Where(i => i.DeleteStatus == false).ToList();          
        }
        public Meeting GetMeetingById(int id)
        {
            return db.Meetings.Include("User").Include("Client").Include("Case").FirstOrDefault(i => i.Id == id);
        }
        public List<Meeting> SearchMeeting(string searchTerm = null, DateTime? meetingDate = null)
        {
            var query = db.Meetings
                .Include("User").Include(c => c.Client)
                .Include(c => c.Case)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Client.FullName.Contains(searchTerm));
            }

            if (meetingDate.HasValue)
            {
                var startDate = meetingDate.Value.Date;
                var endDate = startDate.AddDays(1);

                // این بخش جایگزین خطای قبلیت شده
                query = query.Where(c => c.MeetingDateTime >= startDate && c.MeetingDateTime < endDate);
            }
            return query.ToList();
        }
        public List<Meeting> GetTodayAndPreReminderMeetings()
        {
            var now = DateTime.Now;
            var todayStart = now.Date;

            var potentialReminders = db.Meetings
                .Include("User").Include(c => c.Client)
                .Include(c => c.Case)
                .Where(c => c.IsReminderMeetingSet == true &&
                            c.MeetingDateTime != default &&
                            c.Reminder_Stage != Meeting.ReminderStage.Done) // جدید
                .ToList();

            var reminders = new List<Meeting>();

            foreach (var meeting in potentialReminders)
            {
                var meetingTime = meeting.MeetingDateTime;

                // یک روز قبل
                if (meeting.Reminder_Stage == Meeting.ReminderStage.None &&
                    meetingTime.Date.AddDays(-1) == todayStart)
                {
                    reminders.Add(meeting);
                    meeting.Reminder_Stage = Meeting.ReminderStage.OneDayBefore;
                }
                // یک ساعت قبل
                else if (meeting.Reminder_Stage == Meeting.ReminderStage.OneDayBefore &&
                         (meetingTime - now).TotalMinutes >= 0 &&
                         (meetingTime - now).TotalMinutes <= 60)
                {
                    reminders.Add(meeting);
                    meeting.Reminder_Stage = Meeting.ReminderStage.Done;
                    meeting.IsReminderMeetingSet = false; // چون دیگه نیاز به یادآوری نیست
                }
            }

            db.SaveChanges();
            return reminders;
        }
        public List<Meeting> GetReminders()
        {
            return db.Meetings
                     .Where(m => m.IsReminderMeetingSet && !m.IsReminderMeetingDone)
                     .ToList();
        }
    }
}