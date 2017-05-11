using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.Document
{
    public class DocumentEventAccesses
    {
        public int Id { get; set; }
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_EntityTypeId", 1)]
        public int EntityTypeId { get; set; }
        public int DocumentId { get; set; }
        public int EventId { get; set; }
        public int AccessTypeId { get; set; }
        public Nullable<int> PositionId { get; set; }
        public Nullable<int> AgentId { get; set; }
        public int? PositionExecutorTypeId { get; set; }

        public Nullable<DateTime> SendDate { get; set; }
        public Nullable<DateTime> ReadDate { get; set; }
        public Nullable<int> ReadAgentId { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsAddLater { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public int? CountNewEvents { get; set; }
        public int? CountWaits { get; set; }
        public int? OverDueCountWaits { get; set; }
        public DateTime? MinDueDate { get; set; }


        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("EventId")]
        public virtual DocumentEvents Event { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("PositionExecutorTypeId")]
        public virtual DictionaryPositionExecutorTypes PositionExecutorType { get; set; }
        [ForeignKey("ReadAgentId")]
        public virtual DictionaryAgents ReadAgent { get; set; }
    }
}
