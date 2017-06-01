using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClients
    {
        public AspNetClients()
        {
            this.Licences = new HashSet<AspNetClientLicences>();
            this.Servers = new HashSet<AspNetClientServers>();
            this.Invitations = new HashSet<AspNetClientInvitations>();
        }
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }

        [MaxLength(256)]
        [Index("IX_Code", IsUnique = true)]
        public string Code { get; set; }

        [MaxLength(2000)]
        public string VerificationCode { get; set; }

        public int LanguageId { get; set; }


        [ForeignKey("LanguageId")]
        public virtual AdminLanguages Language { get; set; }

        [ForeignKey("ClientId")]
        public virtual ICollection<AspNetClientLicences> Licences { get; set; }

        [ForeignKey("ClientId")]
        public virtual ICollection<AspNetClientServers> Servers { get; set; }

        [ForeignKey("ClientId")]
        public virtual ICollection<AspNetClientInvitations> Invitations { get; set; }


    }
}
