using BE;
using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class Dashboard_BLL
    {
        Dashboard_DAL dashboard_DAL = new Dashboard_DAL();      
        public List<Reminder> UserReminderCount(User user) => dashboard_DAL.UserReminderCount(user);

        public string ClientsCount()
        {
            return dashboard_DAL.ClientsCount();
        }    
        public string InteractionsCount()
        {
            return dashboard_DAL.InteractionsCount();
        }
        public string TotalCasesCount()
        {
            return dashboard_DAL.TotalCasesCount();
        }
        public string ActiveCasesCount()
        {
            return dashboard_DAL.ActiveCasesCount();
        }
        public string ClosedCasesCount()
        {
            return dashboard_DAL.ClosedCasesCount();
        }
        public string CorrespondenceCount()
        {
            return dashboard_DAL.CorrespondenceCount();
        }
    }
}