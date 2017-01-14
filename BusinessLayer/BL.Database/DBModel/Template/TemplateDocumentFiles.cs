using BL.Database.DBModel.Dictionary;
using BL.Model.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocumentFiles
    {
        public int Id { get; set; }
        [Index("IX_DocumentNameExtention", 1, IsUnique = true)]
        [Index("IX_DocumentOrderNumber", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        [MaxLength(200)]
        [Index("IX_DocumentNameExtention", 2, IsUnique = true)]
        public string Name { get; set; }
        [Index("IX_DocumentOrderNumber", 2, IsUnique = true)]
        public int OrderNumber { get; set; }
        [MaxLength(200)]
        [Index("IX_DocumentNameExtention", 3, IsUnique = true)]
        public string Extention { get; set; }
        [MaxLength(2000)]
        public string FileType { get; set; }
        public long FileSize { get; set; }
        //public byte[] Content { get; set; }
        public int TypeId { get; set; }
        [MaxLength(2000)]
        public string Hash { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool? IsPdfCreated { get; set; }
        public DateTime? LastPdfAccessDate { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual TemplateDocuments Document { get; set; }
        [ForeignKey("TypeId")]
        public virtual DictionaryFileTypes Type { get; set; }
    }
}
