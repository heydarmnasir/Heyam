using System;
using System.Globalization;
using System.Windows.Media;

namespace BE.ViewModel
{
    public class SMSPanelDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }      
        public string SenderLineNumber { get; set; }
        public string ShortGroupId { get; set; }
        public int UserRole { get; set; }
        public bool IsSelected { get; set; } // 👈 یادت نره اضافه کنی!
        public bool IsSuccess { get; set; }
        public string StatusText => IsSuccess ? "موفق" : "ناموفق";
        public Brush StatusColor => IsSuccess ? Brushes.ForestGreen : Brushes.Crimson;

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
                return $"{pc.GetYear(SentAt):0000}/{pc.GetMonth(SentAt):00}/{pc.GetDayOfMonth(SentAt):00}";
            }
        }
    }
}