using BL.Model.Enums;
using BL.Model.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontSystemLog
    {
        public int? Id { get; set; }
        public int? ClientId { get; set; }
        public int? LogLevel { get; set; }
        public string LogLevelName { get; set; }
        public string Message { get; set; }
        public string LogTrace { get; set; }
        public string LogException { get; set; }
        public string ObjectLog { get; set; }
        public object LogObject { get; set; }
        public int? ExecutorAgentId { get; set; }
        public string ExecutorAgent { get; set; }
		
        private DateTime  _LogDate; 
        public DateTime LogDate { get { return _LogDate; } set { _LogDate=value.ToUTC(); } }
		
        public int? ObjectId { get; set; }
        public string ObjectName { get; set; }
        public int? ActionId { get; set; }
        public string ActionName { get; set; }
        public int? RecordId { get; set; }
    }
}
