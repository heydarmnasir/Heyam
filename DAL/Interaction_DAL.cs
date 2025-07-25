using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DAL
{
    public class Interaction_DAL
    {
        DB db = new DB();

        public int Create(Interaction interaction)
        {
            db.Interactions.Add(interaction);
            db.SaveChanges();
            return interaction.Id;
        }
        public List<Interaction> GetAllInteractions()
        {
            return db.Interactions.Include("User").Include("Client").ToList();
        }
        public List<Interaction> SearchByClientName(string fullname)
        {
            return db.Interactions
                     .Include("User").Include("Client")
                     .Where(i => i.Client.FullName.Contains(fullname))
                     .ToList();
        }
        public Interaction GetInteractionById(int id)
        {
            return db.Interactions.Include("User").Include("Client").FirstOrDefault(i => i.Id == id);
        }
        public string UpdateInteraction(Interaction updated)
        {
            var existing = db.Interactions.Find(updated.Id);
            if (existing != null)
            {
                existing.Note = updated.Note;
                existing.IsReminderSet = updated.IsReminderSet;
                existing.ReminderDate = updated.ReminderDate;                
                existing.IsReminderDone = updated.IsReminderDone;
                db.SaveChanges();
                return "ویرایش اطلاعات با موفقیت انجام شد";
            }
            return "خطا در ویرایش اطلاعات!";
        }
        public List<Interaction> GetTodayReminders()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var reminders = db.Interactions
                .Include("User").Include(c => c.Client)
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
        public List<Interaction> GetReminders()
        {
            return db.Interactions
                     .Where(m => m.IsReminderSet && !m.IsReminderDone)
                     .ToList();
        }
    }
}