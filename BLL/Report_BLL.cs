using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BLL
{
    public class Report_BLL
    {
        private readonly Payment_DAL paymentDAL = new Payment_DAL();
        private readonly LawyerExpense_DAL costDAL = new LawyerExpense_DAL();
        private readonly PersianCalendar pc = new PersianCalendar();

        public List<ChartDataPoint> GetDailyChartData_CurrentMonth()
        {
            var now = DateTime.Now;
            int year = pc.GetYear(now);
            int month = pc.GetMonth(now);
            int daysInMonth = pc.GetDaysInMonth(year, month);

            var result = new List<ChartDataPoint>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime from = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
                DateTime to = from.AddDays(1).AddTicks(-1);

                decimal income = paymentDAL.GetTotalIncome(from, to);
                decimal cost = costDAL.GetTotalCost(from, to);

                result.Add(new ChartDataPoint
                {
                    Label = $"{year}/{month:00}/{day:00}",
                    Income = income,
                    Cost = cost
                });
            }

            return result;
        }

        public List<ChartDataPoint> GetMonthlyData_CurrentYear()
        {
            int year = pc.GetYear(DateTime.Now);
            var list = new List<ChartDataPoint>();

            for (int month = 1; month <= 12; month++)
            {
                DateTime from = pc.ToDateTime(year, month, 1, 0, 0, 0, 0);
                DateTime to = from.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                var income = paymentDAL.GetTotalIncome(from, to);
                var cost = costDAL.GetTotalCost(from, to);

                list.Add(new ChartDataPoint
                {
                    Label = GetPersianMonthName(month),
                    Income = income,
                    Cost = cost
                });
            }

            return list;
        }

        public List<ChartDataPoint> GetYearlyChartData_Range(int fromYear, int toYear)
        {
            var result = new List<ChartDataPoint>();

            for (int year = fromYear; year <= toYear; year++)
            {
                DateTime from = pc.ToDateTime(year, 1, 1, 0, 0, 0, 0);
                DateTime to = pc.ToDateTime(year, 12, 29, 23, 59, 59, 999);

                decimal income = paymentDAL.GetTotalIncome(from, to);
                decimal cost = costDAL.GetTotalCost(from, to);

                result.Add(new ChartDataPoint
                {
                    Label = year.ToString(),
                    Income = income,
                    Cost = cost
                });
            }

            return result;
        }

        private string GetPersianMonthName(int month)
        {
            string[] months = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
                            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            return (month >= 1 && month <= 12) ? months[month - 1] : "نامعتبر";
        }
    }
}