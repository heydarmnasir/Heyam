using BE;
using BE.ViewModel;
using DAL;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class LegalBrief_BLL
    {
        LegalBrief_DAL legalBrief_DAL = new LegalBrief_DAL();
        public int AddBrief(LegalBrief legalBrief) => legalBrief_DAL.AddBrief(legalBrief);
        public List<LegalBrief> GetAllBrief() => legalBrief_DAL.GetAllBriefs();
        public List<BE.ViewModel.LegalBriefDto> GetLegalBriefsForListView()
        {
            var LegalBriefs = legalBrief_DAL.GetAllBriefs(); // فرض بر اینکه این متد شامل Join با Client است
            var result = LegalBriefs.Select(i => new BE.ViewModel.LegalBriefDto
            {
                Id = i.Id,
                ClientName = i.Client.FullName,
                CaseNumber = i.Case?.CaseNumber ?? "",
                Title = i.Title,              
                SetDate = i.SetDate.ToString("yyyy/MM/dd"),
                IsDeliverySet = i.IsDeliverySet ? "✅ دارد" : "❌ ندارد",
                DeliveryDate = i.DeliveryDate.HasValue ? i.DeliveryDate.Value.ToString("yyyy/MM/dd") : "",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public LegalBrief GetBriefById(int id) => legalBrief_DAL.GetBriefById(id);            
        public List<LegalBrief> Search(string searchTerm , int? briefTitle) => legalBrief_DAL.Search(searchTerm);
        public List<LegalBriefDto> SearchLegalBriefsForDGV(string searchTerm)
        {
            var LegalBriefs = legalBrief_DAL.Search(searchTerm); // فرض بر اینکه این متد شامل Join با Client است
            var result = LegalBriefs.Select(i => new BE.ViewModel.LegalBriefDto
            {
                Id = i.Id,
                ClientName = i.Client.FullName,
                CaseNumber = i.Case?.CaseNumber ?? "",
                Title = i.Title,              
                SetDate = i.SetDate.ToString("yyyy/MM/dd"),
                IsDeliverySet = i.IsDeliverySet ? "✅ دارد" : "❌ ندارد",
                DeliveryDate = i.DeliveryDate.HasValue ? i.DeliveryDate.Value.ToString("yyyy/MM/dd") : "",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public string UpdateBrief(LegalBrief legalBrief) => legalBrief_DAL.UpdateBrief(legalBrief);      
        public string DeleteBrief(int id) => legalBrief_DAL.DeleteBrief(id);
        public List<LegalBrief> GetTodayReminders() => legalBrief_DAL.GetTodayReminders();
        public List<LegalBriefDto> GetReminders()
        {
            var LegalBriefs = legalBrief_DAL.GetReminders(); // فرض بر اینکه این متد شامل Join با Client است
            var result = LegalBriefs.Select(i => new BE.ViewModel.LegalBriefDto
            {
                Id = i.Id,
                ClientName = i.Client.FullName,
                CaseNumber = i.Case?.CaseNumber ?? "",
                Title = i.Title,
                SetDate = i.SetDate.ToString("yyyy/MM/dd"),
                IsDeliverySet = i.IsDeliverySet ? "✅ دارد" : "❌ ندارد",
                DeliveryDate = i.DeliveryDate.HasValue ? i.DeliveryDate.Value.ToString("yyyy/MM/dd") : "",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
    }
}