using BE;
using System.Data.Entity;

namespace DAL
{
    public class DB : DbContext
    {
        public DB() : base("constring") { }

        public DbSet<User> Users { get; set; }        
        public DbSet<Client> Clients { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseAttachment> CaseAttachments { get; set; }
        public DbSet<Correspondence> Correspondences { get; set; }
        public DbSet<CorrespondenceAttachment> CorrespondenceAttachments { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<LegalBrief> LegalBriefs { get; set; }      
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentAttachment> PaymentAttachments { get; set; }
        public DbSet<LawyerExpense> LawyerExpenses { get; set; }              
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<PersonalNote> PersonalNotes { get; set; }
        public DbSet<SmsPanel> SmsPanels { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contract>()
                .HasRequired(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.ClientId)
                .WillCascadeOnDelete(false); // 👈 این خط باعث میشه حذف آبشاری نباشه

            modelBuilder.Entity<Contract>()
            .Property(p => p.SetDate)
            .HasColumnType("datetime2");
            modelBuilder.Entity<Payment>()
           .Property(p => p.PaymentDate)
           .HasColumnType("datetime2");
            modelBuilder.Entity<LawyerExpense>()
           .Property(p => p.ExpenseDate)
           .HasColumnType("datetime2");
        }
    }
}