using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE
{
    public class LegalBrief
    {
        public int Id { get; set; }    
        public int Title { get; set; }
        public string Content { get; set; }     
        public DateTime SetDate { get; set; }
        public bool IsDeliverySet { get; set; } // آیا یادآوری دارد؟
        public DateTime? DeliveryDate { get; set; }
        public bool IsDeliveryDone { get; set; } // آیا یادآوری انجام شده؟
        public bool DeleteStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        #region Associations
        public int UserRole { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public int? CaseId { get; set; }
        [ForeignKey("CaseId")]
        public virtual Case Case { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }

        #endregion

        #region ConstructorMethod
        public LegalBrief()
        {         
            DeleteStatus = false;
        }
        #endregion
    }

    public class BriefTemplate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
