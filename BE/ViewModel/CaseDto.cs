using System.Windows.Media;

namespace BE.ViewModel
{
    public class CaseDto
    {
        public int Id { get; set; }
        public string OpponentPerson { get; set; }
        public string CaseNumber { get; set; }
        public string CaseArchiveNumber { get; set; }
        public string ClientName { get; set; } // فقط برای نمایش در DataGrid
        public string SetDate { get; set; }
        public string CaseTitle { get; set; }
        public string CaseSubject { get; set; }
        public string ProcessingBranch { get; set; }
        public int CaseStatus { get; set; }
        public int UserRole { get; set; }

        //استایل وضعیت پرونده در دیتا گرید
        public string StatusText
        {
            get
            {
                if (CaseStatus == 0) return "در حال رسیدگی";
                if (CaseStatus == 1) return "مختومه";
                if (CaseStatus == 2) return "معلق";
                return "نامشخص";
            }
        }
        public Brush StatusColor
        {
            get
            {
                if (CaseStatus == 0) return Brushes.Blue;
                if (CaseStatus == 1) return Brushes.ForestGreen;
                if (CaseStatus == 2) return Brushes.Crimson;
                return Brushes.Black;
            }
        }
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
    }
}