using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using BL.Model.Database;
using System.Linq;
using BL.Database.Helpers;
using BL.CrossCutting.DependencyInjection;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int? AgentId { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public ApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString, throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            if(System.Web.HttpContext.Current.Request.Path.Equals("/token"))
            {
                int dbId = int.Parse(System.Web.HttpContext.Current.Request.Headers["DatabaseId"]);
                var readXml = new Utilities.ReadXml("/servers.xml");
                var dbs = readXml.ReadDBs();
                var db = dbs.FirstOrDefault(x => x.Id == dbId);
                if (db==null)
                {
                    throw new System.Exception();
                }
                //var cxt = DmsResolver.Current.Get<UserContext>().Set(db);
                return new ApplicationDbContext(db.ConnectionString);
            }
            else if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["Authorization"]))
            {
                var cxt = DmsResolver.Current.Get<UserContext>().Get();
                return new ApplicationDbContext(cxt.CurrentDB.ConnectionString);
            }
            
            return new ApplicationDbContext();
        }
    }
}