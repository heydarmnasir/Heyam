using BE;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using BE.ViewModel;

namespace DAL
{
    public class SmsPanel_DAL
    {
        DB db = new DB();
        public string Add(SmsPanel sms)
        {
            db.SmsPanels.Add(sms);
            db.SaveChanges();
            return "پیامک با موفقیت ارسال و در سیستم ثبت شد";
        }
        public List<SmsPanel> GetAll()
        {
            return db.SmsPanels.Include(s => s.Client).OrderByDescending(s => s.SentAt).ToList();
        }
        public List<SMSPanelDto> GetAllForGroupClientCheckList()
        {
            return db.Clients
                .Select(c => new SMSPanelDto
                {
                    Id = c.Id,
                    ClientName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    IsSelected = false
                }).ToList();
        }
        public List<SMSPanelDto> SearchClientsForGroupSms(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return db.Clients
                    .Select(c => new SMSPanelDto
                    {
                        Id = c.Id,
                        ClientName = c.FullName,
                        PhoneNumber = c.PhoneNumber,
                        IsSelected = false
                    }).ToList();
            }

            searchText = searchText.Trim();

            return db.Clients
                .Where(c => c.FullName.Contains(searchText) || c.PhoneNumber.Contains(searchText))
                .Select(c => new SMSPanelDto
                {
                    Id = c.Id,
                    ClientName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    IsSelected = false
                }).ToList();
        }
        public List<SmsPanel> SearchByClientName(string searchText)
        {
            return db.SmsPanels
                     .Include("User").Include("Client")
                     .Where(i => i.Client.FullName.Contains(searchText) || i.Client.PhoneNumber.Contains(searchText))
                     .ToList();
        }
        public SmsPanel GetSMSById(int id)
        {
            return db.SmsPanels.Include("User").Include(sms => sms.Client).FirstOrDefault(sms => sms.Id == id);
        }
    }
}