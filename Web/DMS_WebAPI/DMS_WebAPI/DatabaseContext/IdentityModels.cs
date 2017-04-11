using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using DMS_WebAPI.DBModel;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual ICollection<AspNetUserServers> UserServers { get; set; }
        public virtual ICollection<AspNetUserFingerprints> UserFingerprints { get; set; }

        public bool IsChangePasswordRequired { get; set; }
        public bool IsEmailConfirmRequired { get; set; }
        public bool IsLockout { get; set; }

        public bool IsFingerprintEnabled { get; set; }

        public int? ControlQuestionId { get; set; }

        public string ControlAnswer { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastChangeDate { get; set; }

        [ForeignKey("ControlQuestionId")]
        public virtual SystemControlQuestions ControlQuestion { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
        public virtual DbSet<AspNetClientLicences> AspNetClientLicencesSet { get; set; }
        public virtual DbSet<AspNetClientServers> AspNetClientServersSet { get; set; }
        public virtual DbSet<AspNetUserServers> AspNetUserServersSet { get; set; }
        public virtual DbSet<AspNetUserClients> AspNetUserClientsSet { get; set; }
        public virtual DbSet<AspNetUserFingerprints> AspNetUserFingerprintsSet { get; set; }
        public virtual DbSet<AspNetUserContexts> AspNetUserContextsSet { get; set; }
        public virtual DbSet<AspNetLicences> AspNetLicencesSet { get; set; }

        public virtual DbSet<SystemControlQuestions> SystemControlQuestionsSet { get; set; }
    }
}