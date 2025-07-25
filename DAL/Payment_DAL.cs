using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DAL
{
    public class Payment_DAL
    {
        DB db = new DB();
        public int Create(Payment payment)
        {
            db.Payments.Add(payment);
            db.SaveChanges();
            return payment.Id;
        }
        public List<Payment> GetAllPayments()
        {
            return db.Payments.Include("User").Where(i => i.DeleteStatus == false).ToList();
        }
        public Payment GetPaymentId(int id)
        {
            return db.Payments.Include("User").Include("Client").Include("Case").FirstOrDefault(i => i.Id == id);
        }
        public List<Payment> SearchPayments(string searchTerm , int? filterReminder)
        {
            var query = db.Payments
            .Include("User").Include(c => c.Client).Include(c => c.Case).Include(c => c.PaymentAttachments)
            .AsQueryable();
            // فیلتر براساس متن جستجو (در صورتی که مقدار داشته باشد)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                (c.Client != null && c.Client.FullName.Contains(searchTerm)) ||
                (c.Case != null && c.Case.CaseSubject.Contains(searchTerm)));

            }
            if (filterReminder.HasValue)
            {
                query = query.Where(c => c.PaymentStatus == filterReminder.Value);
            }
            return query.ToList();
        }       
        public bool UpdatePayments(Payment updatedPayment)
        {
            try
            {
                var existingPayment = db.Payments
               .Include("User").Include(c => c.Client).Include(c => c.Case).Include(c => c.PaymentAttachments)
               .FirstOrDefault(c => c.Id == updatedPayment.Id);

                if (existingPayment == null) return false;

                // به‌روزرسانی فیلدهاs
                db.Entry(existingPayment).CurrentValues.SetValues(updatedPayment);

                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }        
        }
        public string Delete(int PaymentId)
        {
            var PaymentToRemove = db.Payments.Include("User").Include("Client").Include("Case").Include("PaymentAttachments").FirstOrDefault(c => c.Id == PaymentId);
            if (PaymentToRemove != null)
            {
                db.Payments.Remove(PaymentToRemove);
                db.SaveChanges();
                return "پرداختی موکل موردنظر با موفقیت حذف شد";
            }
            return "خطا در حذف پرداختی موکل!";
        }
        public decimal? GetTotalAmount()
        {
            return db.Payments.Sum(e => e.Amount);
        }
        public int GetCount()
        {
            return db.Payments.Count();
        }
        public List<Payment> GetTodayReminders()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var reminders = db.Payments
                .Include("User").Include(c => c.Client).Include(c => c.Case).Include(c => c.PaymentAttachments)
                .Where(c => c.IsReminderSet == true &&
                            c.DueDate.HasValue &&
                            c.DueDate >= today &&
                            c.DueDate < tomorrow &&
                            c.IsReminderDone == false).ToList();

            foreach (var reminder in reminders)
            {
                reminder.IsReminderDone = true;
                reminder.IsReminderSet = false;
            }
            db.SaveChanges(); // ذخیره تغییرات در دیتابیس
            return reminders;
        }
        public List<Payment> GetReminders()
        {
            return db.Payments
                     .Where(m => m.IsReminderSet && !m.IsReminderDone)
                     .ToList();
        }
        public decimal GetTotalIncome(DateTime from, DateTime to)
        {
            return db.Payments
                     .Where(p => p.PaymentDate >= from && p.PaymentDate <= to)
                     .Sum(p => (decimal?)p.Amount) ?? 0;
        }
    }
}