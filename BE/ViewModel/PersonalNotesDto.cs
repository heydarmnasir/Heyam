using System;
using System.Globalization;
using System.Windows.Media;

namespace BE.ViewModel
{
    public class PersonalNotesDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string IsReminderSet { get; set; }
        public DateTime ReminderDate { get; set; }
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
                return $"{pc.GetYear(ReminderDate):0000}/{pc.GetMonth(ReminderDate):00}/{pc.GetDayOfMonth(ReminderDate):00}";
            }
        }
    }
}