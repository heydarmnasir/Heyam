using System;
using System.Globalization;
using System.Windows.Media;

namespace BE.ViewModel
{
    public class LawyerExpenseDto
    {
        // فقط برای نمایش در DataGrid
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal? Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string ExpenseType { get; set; }
        public string Description { get; set; }

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
                return $"{pc.GetYear(ExpenseDate):00}/{pc.GetMonth(ExpenseDate):00}/{pc.GetDayOfMonth(ExpenseDate):00}";
            }
        }
    }
}