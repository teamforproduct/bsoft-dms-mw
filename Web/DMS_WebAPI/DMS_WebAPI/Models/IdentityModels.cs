using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using BL.Logic.DependencyInjection;
using DMS_WebAPI.Utilities;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using System.Data.Entity;
using BL.Model.Database;
using DMS_WebAPI.DBModel;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;

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
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public virtual DbSet<AdminLanguages> AdminLanguagesSet { get; set; }
        public virtual DbSet<AdminLanguageValues> AdminLanguageValuesSet { get; set; }
        public virtual DbSet<AdminServers> AdminServersSet { get; set; }
        public virtual DbSet<DBModel.AspNetClients> AspNetClientsSet { get; set; }
        public virtual DbSet<DBModel.AspNetUserServers> AspNetUserServersSet { get; set; }
        public virtual DbSet<DBModel.AspNetLicences> AspNetLicencesSet { get; set; }
    }
}