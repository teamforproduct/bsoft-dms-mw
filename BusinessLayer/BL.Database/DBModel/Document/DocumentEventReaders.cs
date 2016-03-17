using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.Document
{
    public class DocumentEventReaders
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Nullable<int> PositionId { get; set; }
        public Nullable<int> AgentId { get; set; }
        public Nullable<DateTime> SendDate { get; set; }
        public Nullable<DateTime> ReadDate { get; set; }
        public Nullable<int> ReadAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("EventId")]
        public virtual DocumentEvents Event { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("ReadAgentId")]
        public virtual DictionaryAgents ReadAgent { get; set; }
    }
}
