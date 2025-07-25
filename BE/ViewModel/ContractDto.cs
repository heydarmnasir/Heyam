using System;
using System.Globalization;
using System.Windows.Media;

namespace BE.ViewModel
{
    public class ContractDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string CaseNumber { get; set; }
        public string CaseSubject { get; set; }
        public int ContractType { get; set; }  
        public decimal? TotalAmount { get; set; }
        public DateTime SetDate { get; set; }
        public DateTime CreatedAt { get; set; }
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
        public string ContractTypeText
        {
            get
            {
                if (ContractType == 0) return "قرارداد مشاوره";
                if (ContractType == 1) return "قرارداد وکالت";           
                return "نامشخص";
            }
        }
        public Brush ContractTypeColor
        {
            get
            {
                if (ContractType == 0) return Brushes.Blue;
                if (ContractType == 1) return Brushes.ForestGreen;               
                return Brushes.Black;
            }
        }      
        public string TotalAmountFormat
        {
            get
            {
                return TotalAmount.HasValue
                    ? string.Format("{0:N0} ریال", TotalAmount.Value)
                    : "0 ریال";
            }
        }

        public string SetDateShamsi
        {
            get
            {
                PersianCalendar pc = new PersianCalendar();
                return $"{pc.GetYear(SetDate):00}/{pc.GetMonth(SetDate):00}/{pc.GetDayOfMonth(SetDate):00}";
            }
        }
    }
}