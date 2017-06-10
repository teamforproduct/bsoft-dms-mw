using BL.Model.Enums;
using System;

namespace DMS_WebAPI.DBModel
{
    public class AddSessionLog : SessionEnviroment
    {
        public DateTime Date { get; set; }

        public EnumLogTypes Type { get; set; }

        public string Event { get; set; }

        public string Message { get; set; }

        public string UserId { get; set; }

        public DateTime? LastUsage { get; set; }

    }
}