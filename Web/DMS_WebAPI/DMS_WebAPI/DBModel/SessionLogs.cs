using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class SessionLogs
    {
        public int Id { get; set; }

        [Index("IX_LogDate", 1)]
        public DateTime LogDate { get; set; }

        public int Type { get; set; }

        [MaxLength(40)]
        public string Event { get; set; }

        [MaxLength(2000)]
        public string Message { get; set; }


        [Index("IX_UserId", 1)]
        public string UserId { get; set; }

        public DateTime? LastUsage { get; set; }

        public bool Enabled { get; set; }


        public string IP { get; set; }

        public string Platform { get; set; }

        public string Browser { get; set; }

        public string Fingerprint { get; set; }


        [ForeignKey("UserId")]
        public virtual AspNetUsers User { get; set; }
    }
}