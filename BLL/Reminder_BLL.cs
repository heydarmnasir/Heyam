using BE;
using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class Reminder_BLL
    {
        Reminder_DAL Reminder_dal = new Reminder_DAL();
        public void Create(Reminder reminder) => Reminder_dal.Create(reminder);
        public List<Reminder> GetAllReminders() => Reminder_dal.GetAllReminders();
    }
}