namespace BE
{
    public static class AccessControlService
    {
        public static bool CanAccessUserManagement(User user)
        {
            return user.Role == User.UserRole.Lawyer;
        }
        public static bool CanAccessBackup(User user)
        {
            return user.Role == User.UserRole.Lawyer;
        }        
    }
}