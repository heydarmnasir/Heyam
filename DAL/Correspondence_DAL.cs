using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.IO;

namespace DAL
{
    public class Correspondence_DAL
    {
        DB db = new DB();
        public int AddCorrespondence(Correspondence correspondence)
        {
            db.Correspondences.Add(correspondence);
            db.SaveChanges();
            return correspondence.Id;
        }
        public List<Correspondence> GetAllCorrespondences()
        {
            return db.Correspondences.Include("User").Include(c => c.CorrespondenceAttachments).Include(c => c.Case).Include(c => c.Case.Client).ToList();
        }
        public List<Correspondence> GetCorrespondencesByCaseId(int caseId)
        {
            return db.Correspondences
                           .Include(c => c.CorrespondenceAttachments).Include("User")
                           .Where(c => c.CaseId == caseId)
                           .ToList();
        }
        public Correspondence GetCorrespondenceById(int id)
        {
            return db.Correspondences.Include("User").Include("Case").Include("CorrespondenceAttachments").FirstOrDefault(i => i.Id == id);
        }        
        public List<Correspondence> SearchCorrespondence(string searchTerm, int? statusFilter)
        {
            var query = db.Correspondences
            .Include(c => c.Case).Include(c => c.Case.Client).Include("User")
            .AsQueryable();

            // فیلتر براساس متن جستجو (در صورتی که مقدار داشته باشد)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.Case.CaseNumber.Contains(searchTerm) ||
                    c.Case.Client.FullName.Contains(searchTerm) ||
                    c.Title.Contains(searchTerm));
            }

            // فیلتر براساس وضعیت پرونده (در صورتی که مقدار مشخص شده باشد)
            if (statusFilter.HasValue)
            {
                query = query.Where(c => c.Status == statusFilter.Value);
            }
            return query.ToList();
        }
        public bool UpdateCorrespondence(Correspondence updatedCorrespondence, List<CorrespondenceAttachment> deletedCorrespondenceAttachments)
        {
            try
            {
                var existingCorrespondence = db.Correspondences
               .Include(c => c.CorrespondenceAttachments).Include("User")
               .FirstOrDefault(c => c.Id == updatedCorrespondence.Id);

                if (existingCorrespondence == null) return false;

                // به‌روزرسانی فیلدها
                db.Entry(existingCorrespondence).CurrentValues.SetValues(updatedCorrespondence);

                // حذف پیوست‌هایی که در updatedCase نیستند
                var updatedCorrespondenceAttachmentIds = updatedCorrespondence.CorrespondenceAttachments.Select(a => a.Id).ToList();

                // حذف پیوست‌ها
                foreach (var deleted in deletedCorrespondenceAttachments)
                {
                    var existingCorrespondenceAttachment = db.CorrespondenceAttachments.Find(deleted.Id);
                    if (existingCorrespondenceAttachment != null)
                    {
                        db.CorrespondenceAttachments.Remove(existingCorrespondenceAttachment);
                    }

                    if (File.Exists(deleted.FilePath))
                    {
                        File.Delete(deleted.FilePath);
                    }
                }

                // افزودن پیوست‌های جدید
                foreach (var attachment in updatedCorrespondence.CorrespondenceAttachments)
                {
                    if (attachment.Id == 0)
                    {
                        attachment.CorrespondenceId = existingCorrespondence.Id; // ارتباط را مشخص می‌کنیم
                        db.CorrespondenceAttachments.Add(attachment); // اضافه به دیتابیس ✅
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }       
        }
        public string Delete(int caseId)
        {
            var correspondenceToRemove = db.Correspondences.Include("User").Include("CorrespondenceAttachments").FirstOrDefault(c => c.Id == caseId);
            if (correspondenceToRemove != null)
            {
                db.Correspondences.Remove(correspondenceToRemove);
                db.SaveChanges();
                return "مکاتبه موردنظر با موفقیت حذف شد";
            }
            return "خطا در حذف مکاتبه!";
        }
        public List<Correspondence> GetTodayReminders()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var reminders = db.Correspondences
                .Include("User").Include(c => c.Case).Include(ca => ca.Case.Client)
                .Where(c => c.IsReminderSet == true &&
                            c.ReminderDate.HasValue &&
                            c.ReminderDate >= today &&
                            c.ReminderDate < tomorrow &&
                            c.IsReminderDone == false).ToList();

            foreach ( var reminder in reminders )
            {
                reminder.IsReminderDone = true;
                reminder.IsReminderSet = false;                
            }
            db.SaveChanges(); // ذخیره تغییرات در دیتابیس
            return reminders;
        }
        public List<Correspondence> GetReminders()
        {
            return db.Correspondences
                     .Where(m => m.IsReminderSet && !m.IsReminderDone)
                     .ToList();
        }
    }
}