using System;
using System.Collections.Generic;

namespace BE
{
    public class Case
    {    
        public int Id { get; set; }
        public string OpponentPerson { get; set; }
        public string CaseNumber { get; set; }      
        public string CaseArchiveNumber { get; set; }
        public DateTime SetDate { get; set; }
        public string CaseTitle { get; set; }
        public string CaseSubject { get; set; }
        public string ProcessingBranch { get; set; }
        public int CaseStatus { get; set; }
        public string Description { get; set; }
        public string ClosingDate { get; set; }      
        public DateTime? CreatedAt { get; set; }
        public bool DeleteStatus { get; set; }

        // Navigation Properties
        public int UserRole { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public virtual ICollection<CaseAttachment> CaseAttachments { get; set; }

        #region ConstructorMethod
        public Case()
        {
            DeleteStatus = false;
        }
        #endregion
    }
}