using System;
using System.Collections.Generic;

namespace BE
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string FullNameLawyer { get; set; }
        public DateTime RegDate { get; set; }
        public bool DeleteStatus { get; set; }

        // Navigation Properties
        public int UserTypeRole { get; set; }
        public UserRole Role { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Interaction> Interactions { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
        public virtual ICollection<Correspondence> Correspondences { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<LegalBrief> LegalBriefs { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<LawyerExpense> LawyerExpenses { get; set; }
        public virtual ICollection<Meeting> Meetings { get; set; }
        public virtual ICollection<PersonalNote> PersonalNotes { get; set; }

        public enum UserRole
        {
            Lawyer,   // وکیل
            Secretary // منشی
        }      
        #region ConstructorMethod
        public User()
        {
            DeleteStatus = false;
        }
        #endregion
    }
}