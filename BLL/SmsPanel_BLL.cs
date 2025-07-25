using BE;
using BE.ViewModel;
using DAL;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class SmsPanel_BLL
    {
        SmsPanel_DAL SmsPanel_DAL = new SmsPanel_DAL();

        public string Add(SmsPanel smsPanel) => SmsPanel_DAL.Add(smsPanel);              
        public List<SMSPanelDto> GetAllForGroupClientCheckList() => SmsPanel_DAL.GetAllForGroupClientCheckList();
        public List<SMSPanelDto> SearchClientsForGroupSms(string searchText) => SmsPanel_DAL.SearchClientsForGroupSms(searchText);
        public SmsPanel GetSMSById(int id) => SmsPanel_DAL.GetSMSById(id);
        public List<SMSPanelDto> GetAllForSingleSMS()
        {
            var allMessages = SmsPanel_DAL.GetAll();
            var singleMessages = allMessages
                .Where(s => s.BulkGroupId == null) // فقط پیام‌های تکی
                .OrderByDescending(s => s.SentAt)  // مرتب‌سازی از جدید به قدیم
                .Select(s => new SMSPanelDto
                {
                    Id = s.Id,
                    ClientName = s.Client.FullName,
                    PhoneNumber = s.Client.PhoneNumber,
                    Message = s.Message,
                    SentAt = s.SentAt,
                    SenderLineNumber = s.SenderLineNumber,
                    IsSuccess = s.IsSuccess,
                    ShortGroupId = s.BulkGroupId.HasValue ? s.BulkGroupId.Value.ToString().Substring(0, 6) : "-",
                    UserRole = s.UserRole
                })
                .ToList();
            return singleMessages;
        }
        public List<SMSPanelDto> SearchForSingleSMS(string searchname)
        {
            var allMessages = SmsPanel_DAL.SearchByClientName(searchname);
            var singleMessages = allMessages
                .Where(s => s.BulkGroupId == null) // فقط پیام‌های تکی
                .OrderByDescending(s => s.SentAt)  // مرتب‌سازی از جدید به قدیم
                .Select(s => new SMSPanelDto
                {
                    Id = s.Id,
                    ClientName = s.Client.FullName,
                    PhoneNumber = s.Client.PhoneNumber,
                    Message = s.Message,
                    SentAt = s.SentAt,
                    SenderLineNumber = s.SenderLineNumber,
                    IsSuccess = s.IsSuccess,
                    ShortGroupId = s.BulkGroupId.HasValue ? s.BulkGroupId.Value.ToString().Substring(0, 6) : "-",
                    UserRole = s.UserRole
                })
                .ToList();
                return singleMessages;
        }
        public List<SMSPanelDto> GetAllForGroupSMS()
        {
            var allMessages = SmsPanel_DAL.GetAll();
            var groupMessages = allMessages
                .Where(s => s.BulkGroupId != null) // فقط پیام‌های تکی
                .OrderByDescending(s => s.SentAt)  // مرتب‌سازی از جدید به قدیم
                .Select(s => new SMSPanelDto
                {
                    Id = s.Id,
                    ClientName = s.Client.FullName,
                    PhoneNumber = s.Client.PhoneNumber,
                    Message = s.Message,
                    SentAt = s.SentAt,
                    SenderLineNumber = s.SenderLineNumber,
                    IsSuccess = s.IsSuccess,
                    ShortGroupId = s.BulkGroupId.HasValue ? s.BulkGroupId.Value.ToString().Substring(0, 6) : "-",
                    UserRole = s.UserRole
                })
                .ToList();
            return groupMessages;
        }
        public List<SMSPanelDto> SearchForGroupSMS(string searchname)
        {
            var allMessages = SmsPanel_DAL.SearchByClientName(searchname);
            var groupMessages = allMessages
               .Where(s => s.BulkGroupId != null) // فقط پیام‌های تکی
               .OrderByDescending(s => s.SentAt)  // مرتب‌سازی از جدید به قدیم
               .Select(s => new SMSPanelDto
               {
                   Id = s.Id,
                   ClientName = s.Client.FullName,
                   PhoneNumber = s.Client.PhoneNumber,
                   Message = s.Message,
                   SentAt = s.SentAt,
                   SenderLineNumber = s.SenderLineNumber,
                   IsSuccess = s.IsSuccess,
                   ShortGroupId = s.BulkGroupId.HasValue ? s.BulkGroupId.Value.ToString().Substring(0, 6) : "-",
                   UserRole = s.UserRole
               }).ToList();
            return groupMessages;         
        }     
    }
}