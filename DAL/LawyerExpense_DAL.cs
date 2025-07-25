using BE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL
{
    public class LawyerExpense_DAL
    {
        DB db = new DB();
        public string Add(LawyerExpense expense)
        {
            db.LawyerExpenses.Add(expense);
            db.SaveChanges();
            return "اطلاعات با موفقیت ثبت شد";
        }
        public bool Update(LawyerExpense updatedexpense)
        {
            var existingExpenses = db.LawyerExpenses.FirstOrDefault(c => c.Id == updatedexpense.Id);
            if (existingExpenses == null) return false;
            // به‌روزرسانی فیلدهاs
            db.Entry(existingExpenses).CurrentValues.SetValues(updatedexpense);
            db.SaveChanges();
            return true;
        }
        public string Delete(int id)
        {
            var expense = db.LawyerExpenses.Find(id);
            if (expense != null)
            {
                db.LawyerExpenses.Remove(expense);
                db.SaveChanges();
                return "اطلاعات با موفقیت حذف شد";
            }
            return "ردیف موردنظر یافت نشد!";
        }
        public LawyerExpense GetById(int id)
        {
            return db.LawyerExpenses.Include("User").FirstOrDefault(e => e.Id == id);
        }
        public List<LawyerExpense> GetAll()
        {
            return db.LawyerExpenses.Include("User").ToList();
        }
        public List<LawyerExpense> SearchExpenses(string searchTerm)
        {
            var query = db.LawyerExpenses.AsQueryable();
            // فیلتر براساس متن جستجو (در صورتی که مقدار داشته باشد)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Include("User").Where(c =>
                (c.Title != null && c.Title.Contains(searchTerm)));       
            }           
            return query.ToList();
        }     
        public decimal? GetTotalAmount()
        {
            return db.LawyerExpenses.Sum(e => e.Amount);
        }
        public int GetCount()
        {
            return db.LawyerExpenses.Count();
        }
        public decimal GetTotalCost(DateTime from, DateTime to)
        {
            return db.LawyerExpenses
                     .Where(c => c.ExpenseDate >= from && c.ExpenseDate <= to)
                     .Sum(c => (decimal?)c.Amount) ?? 0;
        }
    }
}