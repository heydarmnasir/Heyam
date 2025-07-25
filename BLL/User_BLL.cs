using DAL;
using BE;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class User_BLL
    {
        User_DAL user_dal = new User_DAL();        
        public string Create(User user)
        {
            return user_dal.Create(user);
        }
        public bool Exist(User user)
        {
            return user_dal.Exist(user);
        }
        public User Login(string Username, string Password)
        {
            return user_dal.Login(Username, Password);
        }
        public List<User> ReadAll() => user_dal.ReadAll();
        public List<BE.ViewModel.UserDto> ReadAllForListView()
        {
            var users = user_dal.ReadAll(); // فرض بر اینکه این متد شامل Join با Client است
            var result = users.Select(i => new BE.ViewModel.UserDto
            {
                Id = i.Id,
                FullName = i.FullName,
                Username = i.Username,
                UserTypeRole = i.UserTypeRole
            }).ToList();
            return result;
        }
        public User ReadById(int UserId)
        {
            return user_dal.ReadById(UserId);
        }
        // چک کردن وجود کاربر
        public int? CheckUserExists(string username)
        {
            var user = user_dal.GetUserByUsername(username);
            return user?.Id; // اگر کاربر پیدا شد آیدی رو برمیگردونه، وگرنه null
        }
        // تغییر رمز عبور
        public bool ChangeUserPassword(int userId, string newPassword)
        {
            return user_dal.UpdatePassword(userId, newPassword);
        }
        public User GetUser()
        {
            return user_dal.GetUser();
        }      
    }
}