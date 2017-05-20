using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DMS_WebAPI.DBModel
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class AspNetUsers : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AspNetUsers> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        [ForeignKey("UserId")]
        public virtual ICollection<AspNetUserClientServer> ClientServer { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<AspNetUserFingerprints> Fingerprints { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<AspNetUserContexts> Contexts { get; set; }

        [MaxLength(2000)]
        public string LastName { get; set; }
        [MaxLength(2000)]
        public string FirstName { get; set; }

        public int LanguageId { get; set; }

        public bool IsFingerprintEnabled { get; set; }

        public bool IsChangePasswordRequired { get; set; }

        public int? ControlQuestionId { get; set; }


        [MaxLength(2000)]
        public string ControlAnswer { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastChangeDate { get; set; }

        [ForeignKey("ControlQuestionId")]
        public virtual SystemControlQuestions ControlQuestion { get; set; }

        [ForeignKey("LanguageId")]
        public virtual AdminLanguages Language { get; set; }

    }
}