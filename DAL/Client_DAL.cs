using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using BE;
using System.Data.Entity;
using BE.ViewModel;

namespace DAL
{
    public class Client_DAL
    {
        DB db = new DB();
        private readonly string connectionString = @"Data Source=.\SQLEXPRESS;Database=Heyam_DB;Trusted_Connection=True;";

        public string Create(Client client)
        {
            try
            {
                if (!IsExist(client))
                {
                    db.Clients.Add(client);
                    db.SaveChanges();
                    return "موکل با موفقیت ثبت شد";
                }
                else
                {
                    return "موکل در سیستم موجود میباشد!";
                }
            }
            catch (Exception e)
            {
                return "خطا در ثبت موکل!\n" + e.Message;
            }
        }
        public bool IsExist(Client client)
        {
            return db.Clients.Any(i => i.NationalCode == client.NationalCode);            
        }
        public List<Client> GetAllClients()
        {
            // فرض کنید اینجا اطلاعات موکل‌ها از دیتابیس یا هر منبع دیگه‌ای میاد
           return db.Clients.Include("User").Where(i => i.DeleteStatus == false).ToList();          
        }      
        public List<Client> SearchClients(string searchTerm, string genderFilter)
        {
            var query = db.Clients
            .Include(c => c.User)
            .AsQueryable();
            // فیلتر براساس متن جستجو (در صورتی که مقدار داشته باشد)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                c.FullName.Contains(searchTerm) ||
                c.NationalCode.Contains(searchTerm));
            }
            // فیلتر براساس وضعیت پرونده (در صورتی که مقدار مشخص شده باشد)
            if (!string.IsNullOrWhiteSpace(genderFilter))
            {
                query = query.Where(c => c.Gender == genderFilter);
            }
            return query.ToList();
        }       
        public Client GetClientById(int id)
        {
            return db.Clients.Include("User").FirstOrDefault(i => i.Id == id);
        }
        public bool UpdateClientContactInfo(string nationalCode, string phoneNumber, string email, string address)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Clients 
                         SET PhoneNumber = @PhoneNumber, 
                             Email = @Email, 
                             Address = @Address 
                         WHERE NationalCode = @NationalCode";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@NationalCode", nationalCode);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }       
        public string Delete(int Id)
        {
            try
            {
                var q = db.Clients.Where(i => i.Id == Id).FirstOrDefault();
                if (q != null)
                {
                    db.Clients.Remove(q);
                    //q.DeleteStatus = true;
                    db.SaveChanges();
                    return "حذف موکل با موفقیت انجام شد";
                }
                else
                {
                    return "موکل مورد نظر یافته نشد!";
                }
            }
            catch (Exception e)
            {
                return "خطا در حذف مشتری\n" + e.Message;
            }
        }
        public List<string> ReadFullNames()
        {
            return db.Clients.Where(i => i.DeleteStatus == false).Select(i => i.FullName).ToList();
        }
        public Client ReadByFullName(string FullName)
        {
            return db.Clients.Where(u => u.FullName == FullName).FirstOrDefault();
        }
        public List<Client> ReadClientsList()
        {
            return db.Clients.Where(i => i.DeleteStatus == false).ToList();
        }
    }
}