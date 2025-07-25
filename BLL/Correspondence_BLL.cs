using BE;
using BE.ViewModel;
using DAL;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class Correspondence_BLL
    {
        Correspondence_DAL correspondence_DAL = new Correspondence_DAL();
        public int AddCorrespondence(Correspondence correspondence) => correspondence_DAL.AddCorrespondence(correspondence);
        public List<Correspondence> GetCorrespondencesByCaseId(int caseId) => correspondence_DAL.GetCorrespondencesByCaseId(caseId);
        public List<Correspondence> GetAllCorrespondences() => correspondence_DAL.GetAllCorrespondences();    
        public List<BE.ViewModel.CorrespondenceDto> GetCorrespondencesForListView()
        {
            var correspondences = correspondence_DAL.GetAllCorrespondences(); // فرض بر اینکه این متد شامل Join با Client است
            var result = correspondences.Select(i => new BE.ViewModel.CorrespondenceDto
            {
                Id = i.Id,
                CaseNumber = i.Case.CaseNumber,
                ClientName = i.Case.Client.FullName,
                Type = i.Type,
                Title = i.Title,
                Status = i.Status,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public Correspondence GetCorrespondenceById(int id)
        {
            return correspondence_DAL.GetCorrespondenceById(id);
        }       
        public List<CorrespondenceDto> SearchCorrespondenceForDGV(string searchTerm, int? statusFilter)
        {
            var correspondence = correspondence_DAL.SearchCorrespondence(searchTerm, statusFilter); // فرض بر اینکه این متد شامل Join با Client است
            var result = correspondence.Select(i => new BE.ViewModel.CorrespondenceDto
            {
                Id = i.Id,
                CaseNumber = i.Case.CaseNumber,
                ClientName = i.Case.Client.FullName,
                Type = i.Type,
                Title = i.Title,
                Status = i.Status,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public bool UpdateCorrespondence(Correspondence updatedCorrespondence, List<CorrespondenceAttachment> deletedAttachments)
        {
            return correspondence_DAL.UpdateCorrespondence(updatedCorrespondence, deletedAttachments);
        }
        public string Delete(int caseId)
        {
            return correspondence_DAL.Delete(caseId);
        }
        public List<Correspondence> GetTodayReminders()
        {
            return correspondence_DAL.GetTodayReminders();
        }
        public List<CorrespondenceDto> GetReminders()
        {
            var correspondences = correspondence_DAL.GetReminders(); // فرض بر اینکه این متد شامل Join با Client است
            var result = correspondences.Select(i => new BE.ViewModel.CorrespondenceDto
            {
                Id = i.Id,
                CaseNumber = i.Case.CaseNumber,
                ClientName = i.Case.Client.FullName,
                Type = i.Type,
                Title = i.Title,
                Status = i.Status,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }

    }
}