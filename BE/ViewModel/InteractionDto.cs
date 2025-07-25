using System.Windows.Media;

namespace BE.ViewModel
{
    public class InteractionDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string Type { get; set; }
        public string InteractionDate { get; set; }
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
    }
}