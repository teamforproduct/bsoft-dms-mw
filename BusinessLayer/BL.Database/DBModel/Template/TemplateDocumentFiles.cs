using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocumentFiles
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        public int OrderNumber { get; set; }
        [MaxLength(2000)]
        public string Extention { get; set; }
        [MaxLength(2000)]
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public byte[] Content { get; set; }
        public bool IsAdditional { get; set; }
        [MaxLength(2000)]
        public string Hash { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual TemplateDocuments Document { get; set; }
    }
}
