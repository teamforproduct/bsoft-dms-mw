using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentAccesses
    {
        public int Id { get; set; }
        [Index("IX_DocumentPosition", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        [Index("IX_DocumentPosition", 2, IsUnique = true)]
        [Index("IX_PositionId", 1)]
        public int PositionId { get; set; }
        public int AccessLevelId { get; set; }
        public bool IsInWork { get; set; }
        public bool IsFavourite { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
        [ForeignKey("AccessLevelId")]
        public virtual AdminAccessLevels AccessLevel { get; set; }
    }
}
