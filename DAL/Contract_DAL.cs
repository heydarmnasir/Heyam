using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DAL
{
    public class Contract_DAL
    {
        DB db = new DB();
        public string Create(Contract contract)
        {
            try
            {
                db.Contracts.Add(contract);
                db.SaveChanges();
                return "قرارداد با موفقیت ثبت شد";
            }
            catch (Exception)
            {
                return "خطا در ثبت قرارداد!";
            }
        }
        public List<Contract> GetAllContracts()
        {
            return db.Contracts.Include("User").Include(i => i.Client).Include("Case").Where(i => i.DeleteStatus == false).ToList();
        }
        public Contract GetContractId(int id)
        {
            return db.Contracts.Include("User").Include("Client").Include("Case").FirstOrDefault(i => i.Id == id);
        }
        public List<Contract> SearchContracts(string searchTerm, int? typeFilter)
        {
            var query = db.Contracts
            .Include("User").Include(c => c.Client).Include(c => c.Case)
            .AsQueryable();
            // فیلتر براساس متن جستجو (در صورتی که مقدار داشته باشد)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.Client.FullName.Contains(searchTerm) ||
                    c.Case.CaseNumber.Contains(searchTerm));                  
            }
            // فیلتر براساس وضعیت پرونده (در صورتی که مقدار مشخص شده باشد)
            if (typeFilter.HasValue)
            {
                query = query.Where(c => c.ContractType == typeFilter.Value);
            }
            return query.ToList();
        }
        public bool UpdateContracts(Contract updatedContract)
        {
            var existingContract = db.Contracts
                .Include("User").Include(c => c.Client).Include(c => c.Case)
                .FirstOrDefault(c => c.Id == updatedContract.Id);

            if (existingContract == null) return false;

            // به‌روزرسانی فیلدهاs
            db.Entry(existingContract).CurrentValues.SetValues(updatedContract);
           
            db.SaveChanges();
            return true;
        }
        public string Delete(int contractId)
        {
            var contractToRemove = db.Contracts.Include("User").Include("Client").Include("Case").FirstOrDefault(c => c.Id == contractId);
            if (contractToRemove != null)
            {
                db.Contracts.Remove(contractToRemove);
                db.SaveChanges();
                return "قرارداد موردنظر با موفقیت حذف شد";
            }
            return "خطا در حذف قرارداد!";
        }
        public int GetCount()
        {
            return db.Contracts.Count();
        }
        public decimal GetTotalAmount()
        {
            return db.Contracts.Sum(i => (decimal?)i.TotalAmount) ?? 0;
        }
    }
}