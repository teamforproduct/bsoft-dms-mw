using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClientInvitations
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        [MaxLength(256)]
        public string UserEmail { get; set; }


        [ForeignKey("ClientId")]
        public virtual AspNetClients Client { get; set; }


    }
}
