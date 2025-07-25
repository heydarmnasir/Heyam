using System.Windows.Media;

namespace BE.ViewModel
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public string NationalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public int UserRole { get; set; }
        public bool IsSelected { get; set; }
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