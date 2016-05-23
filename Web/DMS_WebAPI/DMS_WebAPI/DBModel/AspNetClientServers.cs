using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetClientServers
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public int ServerId { get; set; }

        [ForeignKey("ClientId")]
        public AspNetClients Client { get; set; }

        [ForeignKey("ServerId")]
        public virtual AdminServers Server { get; set; }
    }
}
