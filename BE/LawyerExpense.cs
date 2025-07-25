using System;

namespace BE
{
    public class LawyerExpense
    {     
        public int Id { get; set; }
        public string Title { get; set; }      
        public decimal? Amount { get; set; }     
        public DateTime ExpenseDate { get; set; }    
        public string ExpenseType { get; set; }      
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserRole { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}