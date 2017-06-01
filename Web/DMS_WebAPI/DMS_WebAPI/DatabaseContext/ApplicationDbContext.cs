using DMS_WebAPI.DBModel;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DMS_WebAPI.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<AspNetUsers>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
            if (!this.Database.Exists())
            {
                this.Database.Initialize(true);
            }
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public static void CreateDatabaseIfNotExists()
        {
            var isIncorrect = false;
            using (var dbContext = new ApplicationDbContext())
            {
                try
                {
                    dbContext.Database.Connection.Open();   // check the database connection
                }
                catch
                {
                    isIncorrect = true;
                }
            }

            if (isIncorrect)
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    //dbContext.Database.Delete();
                }
                using (var dbContext = new ApplicationDbContext())
                {
                }
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public virtual DbSet<AdminLanguages> AdminLanguagesSet { get; set; }
        //public virtual DbSet<AdminLanguageValues> AdminLanguageValuesSet { get; set; }
        public virtual DbSet<AdminServers> AdminServersSet { get; set; }


        public virtual DbSet<AspNetClients> AspNetClientsSet { get; set; }
        public virtual DbSet<AspNetClientRequests> AspNetClientRequestsSet { get; set; }
        
        public virtual DbSet<AspNetClientLicences> AspNetClientLicencesSet { get; set; }
        public virtual DbSet<AspNetClientServers> AspNetClientServersSet { get; set; }
        public virtual DbSet<AspNetClientInvitations> AspNetClientInvitationsSet { get; set; }

        public virtual DbSet<AspNetUserClients> AspNetUserClientServerSet { get; set; }
        public virtual DbSet<AspNetUserFingerprints> AspNetUserFingerprintsSet { get; set; }
        public virtual DbSet<AspNetUserContexts> AspNetUserContextsSet { get; set; }
        public virtual DbSet<AspNetLicences> AspNetLicencesSet { get; set; }

        public virtual DbSet<SystemControlQuestions> SystemControlQuestionsSet { get; set; }
        public virtual DbSet<SystemSettings> SystemSettingsSet { get; set; }
        public virtual DbSet<SystemValueTypes> SystemValueTypesSet { get; set; }

    }
}