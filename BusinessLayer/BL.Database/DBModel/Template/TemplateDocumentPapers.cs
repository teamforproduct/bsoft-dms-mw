using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocumentPapers
    {
        public int Id { get; set; }
        [Index("IX_DocumentNameOrderNumber", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        [MaxLength(400)]
        [Index("IX_DocumentNameOrderNumber", 2, IsUnique = true)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [Index("IX_DocumentNameOrderNumber", 3, IsUnique = true)]
        public bool IsMain { get; set; }
        [Index("IX_DocumentNameOrderNumber", 4, IsUnique = true)]
        public bool IsOriginal { get; set; }
        [Index("IX_DocumentNameOrderNumber", 5, IsUnique = true)]
        public bool IsCopy { get; set; }
        public int PageQuantity { get; set; }
        [Index("IX_DocumentNameOrderNumber", 6, IsUnique = true)]
        public int OrderNumber { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("DocumentId")]
        public virtual TemplateDocuments Document { get; set; }

    }
}
