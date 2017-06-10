using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetUserContexts
    {
        public int Id { get; set; }

        [MaxLength(40)]
        [Index("IX_Key")]
        public string Key { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }
  
        public int ClientId { get; set; }

        [MaxLength(400)]
        public string CurrentPositionsIdList { get; set; }
        
        public DateTime LastChangeDate { get; set; }

        public int? SignInId { get; set; }

        [ForeignKey("SignInId")]
        public SessionLogs Session { get; set; }
        
    }
}