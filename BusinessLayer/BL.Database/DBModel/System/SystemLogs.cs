using System;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.System
{
    public class SystemLogs
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Trace { get; set; }
        public Nullable<int> AgentId { get; set; }
        public DateTime LogDate { get; set; }

        public virtual DictionaryAgents ExecutorAgent { get; set; }
    }
}