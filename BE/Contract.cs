using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE
{
    public class Contract
    {       
        public int Id { get; set; }
        public int ContractType { get; set; }
        public string ContractContent { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime SetDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeleteStatus { get; set; }

        #region Associations
        public int UserRole { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public int? CaseId { get; set; }
        [ForeignKey("CaseId")]
        public virtual Case Case { get; set; }
        #endregion

        #region ConstructorMethod
        public Contract()
        {
            DeleteStatus = false;
        }
        #endregion
    }
}