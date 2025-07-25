using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE
{
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public string NationalCode { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeleteStatus { get; set; }

        //Associations
        public int UserRole { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual ICollection<Interaction> Interactions { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
             
        #region ConstructorMethod
        public Client()
        {
            DeleteStatus = false;
        }
        #endregion
    }
}