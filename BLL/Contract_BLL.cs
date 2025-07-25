using DAL;
using BE;
using System.Collections.Generic;
using System.Linq;
using BE.ViewModel;
using System;

namespace BLL
{
    public class Contract_BLL
    {
        Contract_DAL contract_dal = new Contract_DAL();
        public string Create(Contract contract) => contract_dal.Create(contract);  
        public List<Contract> GetAllContracts() => contract_dal.GetAllContracts();
        public List<BE.ViewModel.ContractDto> GetContractsForListView()
        {
            var contracts = contract_dal.GetAllContracts(); // فرض بر اینکه این متد شامل Join با Client است

            var result = contracts.Select(i => new BE.ViewModel.ContractDto
            {
                Id = i.Id,
                ClientName = i.Client?.FullName ?? "",
                CaseNumber = i.Case?.CaseNumber ?? "", 
                CaseSubject = i.Case?.CaseSubject ?? "",
                ContractType = i.ContractType,
                TotalAmount = i.TotalAmount ?? null,
                SetDate = i.SetDate,
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public Contract GetContractId(int id) => contract_dal.GetContractId(id);
        public List<Contract> SearchContracts(string searchTerm, int? typeFilter) => contract_dal.SearchContracts(searchTerm, typeFilter);
        public List<ContractDto> SearchContractsForDGV(string searchTerm, int? typeFilter)
        {
            var contracts = contract_dal.SearchContracts(searchTerm, typeFilter); // فرض بر اینکه این متد شامل Join با Client است
            var result = contracts.Select(i => new BE.ViewModel.ContractDto
            {
                Id = i.Id,
                ClientName = i.Client?.FullName ?? "",
                CaseNumber = i.Case?.CaseNumber ?? "",
                CaseSubject = i.Case?.CaseSubject ?? "",
                ContractType = i.ContractType,
                TotalAmount = i.TotalAmount ?? null,
                SetDate = i.SetDate,
                UserRole = i.UserRole
            }).ToList();            
            return result;
        }
        public bool UpdateContracts(Contract updatedContract) => contract_dal.UpdateContracts(updatedContract);        
        public string Delete(int contractId) => contract_dal.Delete(contractId);    
        public int GetCount() => contract_dal.GetCount();
        public decimal GetTotalAmount() => contract_dal.GetTotalAmount();
        public List<ContractDto> GetByDateRange(DateTime start, DateTime end)
        {
            var data = contract_dal.GetAllContracts()
                .Where(c => c.SetDate >= start && c.SetDate <= end)
                .Select(c => new ContractDto
                {
                    Id = c.Id,
                    ClientName = c.Client?.FullName ?? "",
                    CaseNumber = c.Case?.CaseNumber ?? "",
                    CaseSubject = c.Case?.CaseSubject ?? "",
                    ContractType = c.ContractType,
                    TotalAmount = c.TotalAmount ?? null,
                    SetDate = c.SetDate,
                    UserRole = c.UserRole
                })
                .ToList();
            return data;
        }
        public int GetCountByDateRange(DateTime start, DateTime end)
        {
            return GetByDateRange(start, end).Count();
        }
        public decimal GetTotalAmountByDateRange(DateTime start, DateTime end)
        {
            return GetByDateRange(start, end)
                      .Sum(c => (decimal?)c.TotalAmount) ?? 0;
        }
    }
}