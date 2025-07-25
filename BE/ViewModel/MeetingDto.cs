using System;
using System.Globalization;
using System.Windows.Media;

namespace BE.ViewModel
{
    public class MeetingDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } // فقط برای نمایش در DataGrid       
        public string MeetingSubject { get; set; }
        public DateTime MeetingDateTime { get; set; }
        public string IsReminderNextMeetingSet { get; set; }
        public int UserRole { get; set; }
        public string UserTypeText
        {
            get
            {
                if (UserRole == 0) return "وکیل";
                if (UserRole == 1) return "منشی";
                return "نامشخص";
            }
        }
        public Brush UserTypeColor
        {
            get
            {
                if (UserRole == 0) return Brushes.Blue;
                if (UserRole == 1) return Brushes.ForestGreen;
                return Brushes.Black;
            }
        }
        public string SetDateShamsi
        {
            get
            {
                PersianCalendar pc = new PersianCalendar();
                return $"{pc.GetYear(MeetingDateTime):0000}/{pc.GetMonth(MeetingDateTime):00}/{pc.GetDayOfMonth(MeetingDateTime):00} " +
                      $"{MeetingDateTime.Hour:00}:{MeetingDateTime.Minute:00}:{MeetingDateTime.Second:00}";
            }
        }
    }
}
