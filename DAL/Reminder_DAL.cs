using BE;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class Reminder_DAL
    {
        DB db = new DB();
        public void Create(Reminder reminder)
        {
            db.Reminders.Add(reminder);
            db.SaveChanges();
        }
        public List<Reminder> GetAllReminders()
        {
            return db.Reminders.ToList();
        }
    }
}