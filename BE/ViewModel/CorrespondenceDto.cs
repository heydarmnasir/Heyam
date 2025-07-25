using System.Windows.Media;

namespace BE.ViewModel
{
    public class CorrespondenceDto
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; }
        public string ClientName { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
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

        //استایل وضعیت مکاتبه در دیتا گرید
        public string StatusText
        {
            get
            {
                if (Status == 0) return "در حال پیگیری";
                if (Status == 1) return "پاسخ دریافت شده";
                if (Status == 2) return "بسته شده";
                return "نامشخص";
            }
        }
        public Brush StatusColor
        {
            get
            {
                if (Status == 0) return Brushes.Blue;
                if (Status == 1) return Brushes.ForestGreen;
                if (Status == 2) return Brushes.Crimson;
                return Brushes.Black;
            }
        }
    }
}
