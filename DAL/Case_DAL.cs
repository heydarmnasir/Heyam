using System.Collections.Generic;
using System.IO;
using System.Linq;
using BE;
using System.Data.Entity;

namespace DAL
{
    public class Case_DAL
    {
        DB db = new DB();
        public string Create(Case caseItem)
        {
            db.Cases.Add(caseItem);
            db.SaveChanges();
            return "پرونده با موفقیت ثبت شد";
        }
        public bool IsExist(Case _case)
        {
            return db.Cases.Any(i => i.CaseNumber == _case.CaseNumber);
        }
        public List<Case> GetAllCases()
        {
            return db.Cases.Include("User").Include(c => c.CaseAttachments).Include(c => c.Client).ToList();
        }
        public Case GetCasesForLegalBriefTemplates(string casenumber)
        {
            return db.Cases.Include("User").Include(c => c.CaseAttachments).Include(c => c.Client).FirstOrDefault(c => c.CaseNumber == casenumber);
        }
        public List<Case> SearchCases(string searchTerm, int? statusFilter)
        {          
            var query = db.Cases
            .Include(c => c.Client).Include("User")
            .AsQueryable();
            // فیلتر براساس متن جستجو (در صورتی که مقدار داشته باشد)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.CaseTitle.Contains(searchTerm) ||
                    c.CaseNumber.Contains(searchTerm) ||
                    c.Client.FullName.Contains(searchTerm));
            }
            // فیلتر براساس وضعیت پرونده (در صورتی که مقدار مشخص شده باشد)
            if (statusFilter.HasValue)
            {
                query = query.Where(c => c.CaseStatus == statusFilter.Value);
            }
            return query.ToList();
        }
        public Case GetCaseById(int id)
        {
            return db.Cases.Include("User").Include("Client").Include("CaseAttachments").FirstOrDefault(i => i.Id == id);
        }        
        public bool UpdateCase(Case updatedCase, List<CaseAttachment> deletedAttachments)
        {
            try
            {
                var existingCase = db.Cases
                .Include(c => c.CaseAttachments)
                .FirstOrDefault(c => c.Id == updatedCase.Id);

                if (existingCase == null) return false;

                // به‌روزرسانی فیلدها
                db.Entry(existingCase).CurrentValues.SetValues(updatedCase);

                // حذف پیوست‌هایی که در updatedCase نیستند
                var updatedAttachmentIds = updatedCase.CaseAttachments.Select(a => a.Id).ToList();

                // حذف پیوست‌ها
                foreach (var deleted in deletedAttachments)
                {
                    var existingAttachment = db.CaseAttachments.Find(deleted.Id);
                    if (existingAttachment != null)
                    {
                        db.CaseAttachments.Remove(existingAttachment);
                    }

                    if (File.Exists(deleted.FilePath))
                    {
                        File.Delete(deleted.FilePath);
                    }
                }
                // افزودن پیوست‌های جدید
                foreach (var attachment in updatedCase.CaseAttachments)
                {
                    if (attachment.Id == 0)
                    {
                        attachment.CaseId = existingCase.Id; // ارتباط را مشخص می‌کنیم
                        db.CaseAttachments.Add(attachment); // اضافه به دیتابیس ✅
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }          
        }
        public string Delete(int caseId)
        {
            var caseToRemove = db.Cases.Include("User").Include("CaseAttachments").FirstOrDefault(c => c.Id == caseId);
            if (caseToRemove != null)
            {
                db.Cases.Remove(caseToRemove);
                db.SaveChanges();
                return "پرونده موردنظر با موفقیت حذف شد";
            }
            return "خطا در حذف پرونده!";
        }
    }
}