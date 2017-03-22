using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentAccesses
    {
        public int Id { get; set; }
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_EntityTypeId", 1)]
        public int EntityTypeId { get; set; }
        [Index("IX_PositionDocument", 2, IsUnique = true)]
        [Index("IX_DocumentPosition", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        [Index("IX_DocumentPosition", 2, IsUnique = true)]
        [Index("IX_PositionDocument", 1, IsUnique = true)]
        public int? PositionId { get; set; }
        public int? AgentId { get; set; }
        public int AccessLevelId { get; set; }
        public bool IsInWork { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsAddLater { get; set; }
        public bool IsActive { get; set; }
        public int? CountNewEvents { get; set; }
        public int? CountWaits { get; set; }
        public int? OverDueCountWaits { get; set; }
        public DateTime? MinDueDate { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("AccessLevelId")]
        public virtual AdminAccessLevels AccessLevel { get; set; }
    }
}
