using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AuthLogs
    {
        public int Id { get; set; }

        [Index("IX_LogDate", 1)]
        public DateTime LogDate { get; set; }

        [MaxLength(2000)]
        public string Message { get; set; }

        [MaxLength(2000)]
        public int Type { get; set; }

        [Index("IX_UserId", 1)]
        public string UserId { get; set; }
        
        public DateTime? LastUsage { get; set; }

        public bool Enabled { get; set; }



        [ForeignKey("UserId")]
        public virtual AspNetUsers User { get; set; }
    }
}