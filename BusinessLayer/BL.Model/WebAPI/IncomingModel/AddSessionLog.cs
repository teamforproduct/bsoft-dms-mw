using BL.Model.Enums;
using System;

namespace DMS_WebAPI.DBModel
{
    public class AddSessionLog
    {
        public DateTime Date { get; set; }

        public EnumLogTypes Type { get; set; }

        public string Event { get; set; }

        public string Message { get; set; }

        public string UserId { get; set; }

        public DateTime? LastUsage { get; set; }

        public string IP { get; set; }

        public string Platform { get; set; }

        public string Browser { get; set; }

        public string Fingerprint { get; set; }

    }
}