namespace BE.ViewModel
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public int UserTypeRole { get; set; }

        public string UserTypeText
        {
            get
            {
                if (UserTypeRole == 0) return "وکیل";
                if (UserTypeRole == 1) return "منشی";
                return "نامشخص";
            }
        }
    }
}
