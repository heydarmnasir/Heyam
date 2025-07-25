using BE;
using BE.ViewModel;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class LawyerExpense_BLL
    {
        LawyerExpense_DAL lawyerExpense_dal = new LawyerExpense_DAL();
        public string Add(LawyerExpense lawyerexpense) => lawyerExpense_dal.Add(lawyerexpense);
        public bool Update(LawyerExpense updatedexpense) => lawyerExpense_dal.Update(updatedexpense);
        public string Delete(int id) => lawyerExpense_dal.Delete(id);
        public LawyerExpense GetById(int id) => lawyerExpense_dal.GetById(id);
        public List<LawyerExpense> GetAll() => lawyerExpense_dal.GetAll();
        public List<LawyerExpenseDto> GetAllExpenseForListView()
        {
            var expenses = lawyerExpense_dal.GetAll(); // فرض بر اینکه این متد شامل Join با Client است
            var result = expenses.Select(i => new LawyerExpenseDto
            {
                Id = i.Id,
                Title = i.Title,
                Amount = i.Amount.Value,
                ExpenseDate = i.ExpenseDate,
                ExpenseType = i.ExpenseType,
                Description = i.Description,
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public List<LawyerExpense> SearchExpenses(string searchTerm) => lawyerExpense_dal.SearchExpenses(searchTerm);
        public List<LawyerExpenseDto> SearchExpensesForListView(string searchTerm)
        {
            var expenses = lawyerExpense_dal.SearchExpenses(searchTerm); // فرض بر اینکه این متد شامل Join با Client است
            var result = expenses.Select(i => new LawyerExpenseDto
            {
                Id = i.Id,
                Title = i.Title,
                Amount = i.Amount.Value,
                ExpenseDate = i.ExpenseDate,
                ExpenseType = i.ExpenseType,
                Description = i.Description,
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public decimal? GetTotalAmount() => lawyerExpense_dal.GetTotalAmount();
        public int GetCount() => lawyerExpense_dal.GetCount();
        public List<LawyerExpenseDto> GetByDateRange(DateTime start, DateTime end)
        {
            var data = lawyerExpense_dal.GetAll()
                .Where(i => i.ExpenseDate >= start && i.ExpenseDate <= end)
                .Select(i => new LawyerExpenseDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Amount = i.Amount.Value,
                    ExpenseDate = i.ExpenseDate,
                    ExpenseType = i.ExpenseType,
                    Description = i.Description,
                    UserRole = i.UserRole
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
                      .Sum(c => (decimal?)c.Amount) ?? 0;
        }
    }
}