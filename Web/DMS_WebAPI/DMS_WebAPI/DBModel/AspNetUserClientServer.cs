using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetUserClientServer
    {
        public int Id { get; set; }

        [Index("IX_UserId", 1)]
        [Index("IX_UserClientServer", 1, IsUnique = true)]
        public string UserId { get; set; }

        [Index("IX_ClientId", 1)]
        [Index("IX_UserClientServer", 2, IsUnique = true)]
        public int ClientId { get; set; }

        [Obsolete("!!!", true)]
        [Index("IX_ServerId", 1)]
        [Index("IX_UserClientServer", 3, IsUnique = true)]
        public int ServerId { get; set; }


        [ForeignKey("UserId")]
        public AspNetUsers User { get; set; }

        [ForeignKey("ServerId")]
        public virtual AdminServers Server { get; set; }

        [ForeignKey("ClientId")]
        public virtual AspNetClients Client { get; set; }
    }
}
