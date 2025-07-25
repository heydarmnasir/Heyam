using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DAL
{
    public class User_DAL
    {
        DB db = new DB();
        public string Create(User user)
        {
            try
            {
                if (!Exist(user))
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return "ثبت کاربر با موفقیت انجام شد";
                }
                else
                {
                    return "نام کاربری در سیستم موجود میباشد!";
                }
            }
            catch (Exception)
            {
                return "خطا در ثبت کاربر!";
            }
        }
        public bool Exist(User user)
        {
            return db.Users.Any(i => i.Username == user.Username);
        }
        public User Login(string Username, string Password)
        {
            return db.Users.Where(i => i.Username == Username && i.Password == Password).SingleOrDefault();
        }
        public List<User> ReadAll()
        {
            return db.Users.ToList();           
        }
        public User ReadById(int UserId)
        {            
            return db.Users.Find(UserId);
        }
        // دریافت کاربر بر اساس نام کاربری
        public User GetUserByUsername(string username)
        {
            return db.Users.FirstOrDefault(u => u.Username == username);
        }
        // آپدیت رمز عبور
        public bool UpdatePassword(int userId, string newPassword)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return false;

            user.Password = newPassword;
            db.SaveChanges();
            return true;
        }     
        public List<string> ReadUserNamesList()
        {
            return db.Users.Where(i => i.DeleteStatus == false).Select(i => i.Username).ToList();
        }
        public bool IsRegister()
        {
            return db.Users.Count() > 0;          
        }             
        public string Delete(int Id)
        {
            try
            {
                User u = ReadById(Id);
                //u.DeleteStatus = true;
                db.Users.Remove(u);
                db.SaveChanges();
                return "کاربر مورد نظر حذف شد";
            }
            catch (Exception e)
            {
                return "حذف اطلاعات با مشکل روبرو شد لطفا بررسی کنید:\n" + e.Message;
            }           
        }                  
        public User GetUser()
        {
            return db.Users.FirstOrDefault();
        }       
    }
}