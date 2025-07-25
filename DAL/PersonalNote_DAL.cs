using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DAL
{
    public class PersonalNote_DAL
    {
        DB db = new DB();
        public int Create(PersonalNote personalNote)
        {
            db.PersonalNotes.Add(personalNote);
            db.SaveChanges();
            return personalNote.Id;
        }
        public List<PersonalNote> GetAllPersonalNotes()
        {
            return db.PersonalNotes.Where(i => i.DeleteStatus == false).ToList();
        }
        public PersonalNote GetPersonalNoteId(int id)
        {
            return db.PersonalNotes.Include(i => i.User).FirstOrDefault(i => i.Id == id);
        }
        public List<PersonalNote> Search(string searchTerm)
        {
            var query = db.PersonalNotes.Include("User").AsQueryable();
            // فیلتر براساس متن جستجو (در صورتی که مقدار داشته باشد)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.Title.Contains(searchTerm) ||
                    c.Category.Contains(searchTerm));
            }
            return query.ToList();
        }
        public string Delete(int notesId)
        {
            var notesToRemove = db.PersonalNotes.Include("User").FirstOrDefault(c => c.Id == notesId);
            if (notesToRemove != null)
            {
                db.PersonalNotes.Remove(notesToRemove);
                db.SaveChanges();
                return "یادداشت موردنظر با موفقیت حذف شد";
            }
            return "خطا در حذف یادداشت!";
        }
        public List<PersonalNote> GetTodayReminders()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var reminders = db.PersonalNotes
                .Include(c => c.User)
                .Where(c => c.IsReminderSet == true &&
                            c.ReminderDate.HasValue &&
                            c.ReminderDate >= today &&
                            c.ReminderDate < tomorrow &&
                            c.IsReminderDone == false).ToList();

            foreach (var reminder in reminders)
            {
                reminder.IsReminderDone = true;
                reminder.IsReminderSet = false;
            }
            db.SaveChanges(); // ذخیره تغییرات در دیتابیس
            return reminders;
        }
        public List<PersonalNote> GetReminders()
        {
            return db.PersonalNotes
                     .Where(m => m.IsReminderSet && !m.IsReminderDone)
                     .ToList();
        }
    }
}