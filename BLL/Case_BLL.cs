using BE;
using BE.ViewModel;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class Case_BLL
    {
        Case_DAL case_dal = new Case_DAL();
        public string Create(Case newCase)
        {
            return case_dal.Create(newCase);
        }
        public bool IsExist(Case _case)
        {
            return case_dal.IsExist(_case);
        }
        public List<Case> GetAllCases()
        {
            return case_dal.GetAllCases();
        }
        public Case GetCasesForLegalBriefTemplates(string casenumber)
        {
            return case_dal.GetCasesForLegalBriefTemplates(casenumber);
        }
        public List<BE.ViewModel.CaseDto> GetCasesForListView()
        {
            var cases = case_dal.GetAllCases(); // فرض بر اینکه این متد شامل Join با Client است

            var result = cases.Select(i => new BE.ViewModel.CaseDto
            {
                Id = i.Id,
                CaseNumber = i.CaseNumber,
                ClientName = i.Client.FullName,
                SetDate = i.SetDate.ToString("yyyy/MM/dd"),
                CaseTitle = i.CaseTitle,
                CaseSubject = i.CaseSubject,
                CaseStatus = i.CaseStatus,
                OpponentPerson = i.OpponentPerson,
                CaseArchiveNumber = i.CaseArchiveNumber,
                ProcessingBranch = i.ProcessingBranch,
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public List<CaseDto> SearchCasesForDGV(string searchTerm, int? statusFilter)
        {
            var cases = case_dal.SearchCases(searchTerm,statusFilter); // فرض بر اینکه این متد شامل Join با Client است
            var result = cases.Select(i => new BE.ViewModel.CaseDto
            {
                Id = i.Id,
                CaseNumber = i.CaseNumber,
                ClientName = i.Client.FullName,
                SetDate = i.SetDate.ToString("yyyy/MM/dd"),
                CaseTitle = i.CaseTitle,
                CaseSubject = i.CaseSubject,
                CaseStatus = i.CaseStatus,
                OpponentPerson = i.OpponentPerson,
                CaseArchiveNumber = i.CaseArchiveNumber,
                ProcessingBranch = i.ProcessingBranch,
                UserRole = i.UserRole
            }).ToList();
            return result;
        }          
        public Case GetCaseById(int id)
        {
            return case_dal.GetCaseById(id);
        }       
        public bool UpdateCase(Case updatedCase , List<CaseAttachment> deletedAttachments)
        {
            return case_dal.UpdateCase(updatedCase,deletedAttachments);
        }
        public string Delete(int caseId)
        {
           return case_dal.Delete(caseId);
        }
        public List<CaseDto> GetByDateRange(DateTime start, DateTime end)
        {
            var data = case_dal.GetAllCases()
                .Where(i => i.SetDate >= start && i.SetDate <= end)
                .Select(i => new CaseDto
                {
                    Id = i.Id,
                    CaseNumber = i.CaseNumber,
                    ClientName = i.Client.FullName,
                    SetDate = i.SetDate.ToString("yyyy/MM/dd"),
                    CaseTitle = i.CaseTitle,
                    CaseSubject = i.CaseSubject,
                    CaseStatus = i.CaseStatus,
                    OpponentPerson = i.OpponentPerson,
                    CaseArchiveNumber = i.CaseArchiveNumber,
                    ProcessingBranch = i.ProcessingBranch,
                    UserRole = i.UserRole                  
                })
                .ToList();
            return data;
        }
        public int GetCountByDateRange(DateTime start, DateTime end)
        {
            return GetByDateRange(start, end).Count();
        }
    }
}