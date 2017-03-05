using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentLinks
    {
        public int Id { get; set; }
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_EntityTypeId", 1)]
        public int EntityTypeId { get; set; }
        [Index("IX_DocumentParentDocument", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        [Index("IX_DocumentParentDocument", 2, IsUnique = true)]
        [Index("IX_ParentDocumentId", 1)]
        public int ParentDocumentId { get; set; }
        public int LinkTypeId { get; set; }
        public int? ExecutorPositionId { get; set; }
        [Column("ExecutorPositionExeAgentId")]
        public int ExecutorPositionExecutorAgentId { get; set; }
        [Column("ExecutorPositionExeTypeId")]
        public int? ExecutorPositionExecutorTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("ParentDocumentId")]
        public virtual Documents ParentDocument { get; set; }
        [ForeignKey("ExecutorPositionId")]
        public virtual DictionaryPositions ExecutorPosition { get; set; }

        [ForeignKey("ExecutorPositionExecutorAgentId")]
        public virtual DictionaryAgents ExecutorPositionExecutorAgent { get; set; }
        [ForeignKey("ExecutorPositionExecutorTypeId")]
        public virtual DictionaryPositionExecutorTypes ExecutorPositionExecutorType { get; set; }
        [ForeignKey("LinkTypeId")]
        public virtual DictionaryLinkTypes LinkType { get; set; }
    }
}
