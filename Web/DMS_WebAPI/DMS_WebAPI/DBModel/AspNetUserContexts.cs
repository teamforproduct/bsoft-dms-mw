using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetUserContexts
    {
        public int Id { get; set; }

        [Index("IX_Key")]
        [MaxLength(32)]
        public string Key { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }

        public int ClientId { get; set; }

        [MaxLength(400)]
        public string CurrentPositionsIdList { get; set; }


        public int? LogId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("ClientId")]
        public virtual AspNetClients Client { get; set; }
    }
}