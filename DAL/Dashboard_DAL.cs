using BE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class Dashboard_DAL
    {
        DB db = new DB();

        public List<Reminder> UserReminderCount(User user)
        {
            return db.Reminders
                .Where(r => r.UserId == user.Id && r.ReminderDate == DateTime.Today)
                .OrderBy(r => r.ReminderDate)
                .Take(7)
                .ToList();
        }
        public string ClientsCount()
        {
            return db.Clients.Where(i => i.DeleteStatus == false).Count().ToString();
        }
        public string InteractionsCount()
        {
            return db.Interactions.Where(i => i.DeleteStatus == false).Count().ToString();
        }
        public string TotalCasesCount()
        {          
            return db.Cases.Where(i => i.DeleteStatus == false).Count().ToString();
        }
        public string ActiveCasesCount()
        {
            return db.Cases.Count(i => i.DeleteStatus == false && i.CaseStatus == 0).ToString();
        }
        public string ClosedCasesCount()
        {
            return db.Cases.Count(i => i.DeleteStatus == false && i.CaseStatus == 1).ToString();
        }
        public string CorrespondenceCount()
        {
            return db.Correspondences.Where(i => i.DeleteStatus == false).Count().ToString();
        }      
    }
}