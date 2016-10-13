using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentSavedFilters
    {
        public int Id { get; set; }
        [Index("IX_NameUser", 3, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [Index("IX_NameUser", 2, IsUnique = true)]
        [Index("IX_UserId", 1)]
        public Nullable<int> UserId { get; set; }
        [MaxLength(400)]
        [Index("IX_NameUser", 1, IsUnique = true)]
        public string Name { get; set; }
        [MaxLength(400)]
        public string Icon { get; set; }
        [MaxLength(2000)]
        public string Filter { get; set; }
        public bool IsCommon { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("UserId")]
        public virtual DictionaryAgentUsers User { get; set; }
    }
}
