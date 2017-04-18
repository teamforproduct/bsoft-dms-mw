using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClientServers
    {
        public int Id { get; set; }

        [Index("IX_ClientId", 1)]
        [Index("IX_ClientServer", 1, IsUnique = true)]
        public int ClientId { get; set; }

        [Index("IX_ServerId", 1)]
        [Index("IX_ClientServer", 2, IsUnique = true)]
        public int ServerId { get; set; }



        [ForeignKey("ClientId")]
        public virtual AspNetClients Client { get; set; }

        [ForeignKey("ServerId")]
        public virtual AdminServers Server { get; set; }
    }
}
