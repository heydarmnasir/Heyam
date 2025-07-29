using BE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL
{
    public class LegalBrief_DAL
    {
        DB db = new DB();
        public int AddBrief(LegalBrief legalBrief)
        {
            db.LegalBriefs.Add(legalBrief);
            db.SaveChanges();
            return legalBrief.Id;
        }
        // گرفتن همه لایحه‌ها
        public List<LegalBrief> GetAllBriefs()
        {
            return db.LegalBriefs.Include("User").Include(lb => lb.Client).Include(lb => lb.Case).ToList();
        }    
        // گرفتن لایحه بر اساس Id
        public LegalBrief GetBriefById(int id)
        {
            return db.LegalBriefs.Include("User").Include(lb => lb.Client).Include(lb => lb.Case)
                                      .FirstOrDefault(lb => lb.Id == id);
        }
        // جستجو بر اساس شماره پرونده یا نام موکل
        public List<LegalBrief> Search(string searchTerm)
        {
            return db.LegalBriefs.Include("User").Include(lb => lb.Client).Include(lb => lb.Case)
                .Where(lb => lb.Case.CaseNumber.Contains(searchTerm) || lb.Client.FullName.Contains(searchTerm))
                .ToList();          
        }      
        // به‌روزرسانی لایحه
        public string UpdateBrief(LegalBrief legalBrief)
        {
            try
            {
                db.Entry(legalBrief).State = EntityState.Modified;
                db.SaveChanges();
                return "اطلاعات با موفقیت ویرایش شد";
            }
            catch (Exception ex)
            {
                return "خطا در ویرایش اطلاعات!" + ex;
            }           
        }
        // حذف لایحه
        public string DeleteBrief(int id)
        {
            try
            {
                var entity = db.LegalBriefs.Find(id);
                if (entity != null)
                {
                    db.LegalBriefs.Remove(entity);
                    db.SaveChanges();
                    return "لایحه با موفقیت حذف شد";
                }
                return "لایحه موردنظر یافته نشد!";
            }
            catch (Exception ex)
            {
                return "خطا در حذف لایحه" + ex;
            }
            
        }
        public List<LegalBrief> GetTodayReminders()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var reminders = db.LegalBriefs
                .Include("User").Include(c => c.Client).Include(c => c.Case)
                .Where(c => c.IsDeliverySet == true &&
                            c.DeliveryDate.HasValue &&
                            c.DeliveryDate >= today &&
                            c.DeliveryDate < tomorrow &&
                            c.IsDeliveryDone == false).ToList();

            foreach (var reminder in reminders)
            {
                reminder.IsDeliveryDone = true;
                reminder.IsDeliverySet = false;
            }
            db.SaveChanges(); // ذخیره تغییرات در دیتابیس
            return reminders;
        }
        public List<LegalBrief> GetReminders()
        {
            return db.LegalBriefs
                     .Where(m => m.IsDeliverySet && !m.IsDeliveryDone)
                     .ToList();
        }
    }
}