using System.Windows.Media;

namespace BE.ViewModel
{
    public class LegalBriefDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string CaseNumber { get; set; }           // شماره پرونده برای نمایش
        public int Title { get; set; }                // عنوان لایحه 
        public string SetDate { get; set; }            // تاریخ تنظیم
        public string IsDeliverySet { get; set; }
        public string DeliveryDate { get; set; }      // تاریخ تحویل یا یادآوری
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
        public string LegalBriefTitleText
        {
            get
            {
                if (Title == 0) return "درخواست مطالعه پرونده";
                if (Title == 1) return "استرداد اعتراض به نظریه کارشناسی";
                if (Title == 2) return "استرداد دادخواست";
                if (Title == 3) return "اسقاط حق تجدیدنظرخواهی یا فرجام‌خواهی";
                if (Title == 4) return "اعتراض به بهای خواسته";
                if (Title == 5) return "اظهار به تقدیم دادخواست جلب ثالث";
                if (Title == 6) return "لایحه دفاعیه (در دعاوی حقوقی)";
                if (Title == 7) return "اعتراض به رأی بدوی";
                if (Title == 8) return "دفاعیه در دعاوی کیفری";
                if (Title == 9) return "درخواست استمهال (تمدید مهلت)";
                if (Title == 10) return "درخواست اجرای حکم";
                if (Title == 11) return "معرفی داور";
                if (Title == 12) return "عدم حضور";
                if (Title == 13) return "✍️ لایحه (سفارشی)";
                return "نامشخص";
            }
        }
        public Brush LegalBriefTitleColor
        {
            get
            {              
                return Brushes.Blue;
            }
        }        
    }
}