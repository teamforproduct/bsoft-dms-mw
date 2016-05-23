using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentSavedFilters
    {
        public int Id { get; set; }
        [Index("IX_IconPosition", 3, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_IconPosition", 2, IsUnique = true)]
        [Index("IX_PositionId", 1)]
        public Nullable<int> PositionId { get; set; }
        [MaxLength(2000)]
        [Index("IX_IconPosition", 1, IsUnique = true)]
        public string Icon { get; set; }
        [MaxLength(2000)]
        public string Filter { get; set; }
        public bool IsCommon { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
    }
}
