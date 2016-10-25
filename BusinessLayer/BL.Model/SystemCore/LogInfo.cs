using System;
using BL.Model.Enums;

namespace BL.Model.SystemCore
{
    public class LogInfo
    {
        public EnumLogTypes LogType { get; set; }
        public string Message { get; set; }
        public string LogTrace { get; set; }
        public string LogException { get; set; }
        public string LogObject { get; set; }
        public int AgentId { get; set; }
        public DateTime Date { get; set; }
        public int? ObjectId { get; set; }
        public int? ActionId { get; set; }
    }
}