using System;
using System.Globalization;
using System.Windows.Media;

namespace BE.ViewModel
{
    public class PaymentDto
    {
        // فقط برای نمایش در DataGrid
        public int Id { get; set; } 
        public string ClientName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientCaseNumber { get; set; }
        public string ClientCaseSubject { get; set; }     
        public int Service { get; set; }
        public int PaymentStatus { get; set; }
        public decimal? Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public string IsReminderSet { get; set; }
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
        //استایل وضعیت پرونده در دیتا گرید
        public string ServiceTypeText
        {
            get
            {
                if (Service == 0) return "مشاوره";
                if (Service == 1) return "حق الوکاله";               
                return "نامشخص";
            }
        }
        public Brush ServiceTypeColor
        {
            get
            {
                if (Service == 0) return Brushes.Blue;
                if (Service == 1) return Brushes.ForestGreen;              
                return Brushes.Black;
            }
        }

        public string PaymentStatusTypeText
        {
            get
            {
                if (PaymentStatus == 0) return "پیش پرداخت";
                if (PaymentStatus == 1) return "تسویه شده";
                return "نامشخص";
            }
        }
        public Brush PaymentStatusTypeColor
        {
            get
            {
                if (PaymentStatus == 0) return Brushes.Blue;
                if (PaymentStatus == 1) return Brushes.ForestGreen;
                return Brushes.Black;
            }
        }

        public string SetDateShamsi
        {
            get
            {
                PersianCalendar pc = new PersianCalendar();
                return $"{pc.GetYear(PaymentDate):00}/{pc.GetMonth(PaymentDate):00}/{pc.GetDayOfMonth(PaymentDate):00}";
            }
        }
    }
}