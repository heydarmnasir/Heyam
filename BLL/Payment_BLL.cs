using BE;
using BE.ViewModel;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class Payment_BLL
    {
        Payment_DAL payment_DAL = new Payment_DAL();
        public int Create(Payment payment) => payment_DAL.Create(payment);
        public List<Payment> GetAllPayments() => payment_DAL.GetAllPayments();
        public List<BE.ViewModel.PaymentDto> GetPaymentsForListView()
        {
            var payments = payment_DAL.GetAllPayments(); // فرض بر اینکه این متد شامل Join با Client است
            var result = payments.Select(i => new BE.ViewModel.PaymentDto
            {
                Id = i.Id,
                ClientName = i.Client != null ? (i.Client.FullName ?? "") : "",
                ClientPhoneNumber = i.Client != null ? (i.Client.PhoneNumber ?? "") : "",
                ClientCaseNumber = i.Case != null ? (i.Case.CaseNumber ?? "") : "",
                ClientCaseSubject = i.Case != null ? (i.Case.CaseSubject ?? "") : "",
                Service = i.Service,
                PaymentStatus = i.PaymentStatus,
                Amount = i.Amount.Value,
                PaymentDate = i.PaymentDate,
                PaymentType = i.PaymentType,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public Payment GetPaymentId(int id) => payment_DAL.GetPaymentId(id);       
        public List<PaymentDto> SearchPaymentsForDGV(string searchTerm, int? filterReminder) =>
        payment_DAL.SearchPayments(searchTerm, filterReminder)
         .Select(i => new PaymentDto
         {
             Id = i.Id,
             ClientName = i.Client != null ? (i.Client.FullName ?? "") : "",
             ClientPhoneNumber = i.Client != null ? (i.Client.PhoneNumber ?? "") : "",
             ClientCaseNumber = i.Case != null ? (i.Case.CaseNumber ?? "") : "",
             ClientCaseSubject = i.Case != null ? (i.Case.CaseSubject ?? "") : "",
             Service = i.Service,
             PaymentStatus = i.PaymentStatus,
             Amount = i.Amount.Value,
             PaymentDate = i.PaymentDate,
             PaymentType = i.PaymentType,
             IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
             UserRole = i.UserRole
         }).ToList();
        public bool UpdatePayments(Payment updatedPayment) => payment_DAL.UpdatePayments(updatedPayment);
        public string Delete(int contractId) => payment_DAL.Delete(contractId);
        public decimal? GetTotalAmount() => payment_DAL.GetTotalAmount();
        public int GetCount() => payment_DAL.GetCount(); 
        public List<Payment> GetTodayReminders() => payment_DAL.GetTodayReminders();
        public List<PaymentDto> GetReminders()
        {
            var payments = payment_DAL.GetReminders(); // فرض بر اینکه این متد شامل Join با Client است
            var result = payments.Select(i => new BE.ViewModel.PaymentDto
            {
                Id = i.Id,
                ClientName = i.Client != null ? (i.Client.FullName ?? "") : "",
                ClientPhoneNumber = i.Client != null ? (i.Client.PhoneNumber ?? "") : "",
                ClientCaseNumber = i.Case != null ? (i.Case.CaseNumber ?? "") : "",
                ClientCaseSubject = i.Case != null ? (i.Case.CaseSubject ?? "") : "",
                Service = i.Service,
                PaymentStatus = i.PaymentStatus,
                Amount = i.Amount.Value,
                PaymentDate = i.PaymentDate,
                PaymentType = i.PaymentType,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public List<PaymentDto> GetByDateRange(DateTime start, DateTime end)
        {
            var data = payment_DAL.GetAllPayments()
                .Where(c => c.PaymentDate >= start && c.PaymentDate <= end)
                .Select(c => new PaymentDto
                {
                    Id = c.Id,
                    ClientName = c.Client != null ? (c.Client.FullName ?? "") : "",
                    ClientPhoneNumber = c.Client != null ? (c.Client.PhoneNumber ?? "") : "",
                    ClientCaseNumber = c.Case != null ? (c.Case.CaseNumber ?? "") : "",
                    ClientCaseSubject = c.Case != null ? (c.Case.CaseSubject ?? "") : "",
                    Service = c.Service,
                    PaymentStatus = c.PaymentStatus,
                    Amount = c.Amount.Value,
                    PaymentDate = c.PaymentDate,
                    PaymentType = c.PaymentType,
                    IsReminderSet = c.IsReminderSet ? "✅ دارد" : "❌ ندارد",
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
                      .Sum(c => (decimal?)c.Amount) ?? 0;
        }
    }
}