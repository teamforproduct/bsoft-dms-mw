using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentPapers
    {
        public DocumentPapers()
        {
            this.Events = new HashSet<DocumentEvents>();
        }

        public int Id { get; set; }
        [Index("IX_DocumentNameOrderNumber", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        [MaxLength(2000)]
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
        public bool IsInWork { get; set; }
        public int? LastPaperEventId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("PaperId")]
        public virtual ICollection<DocumentEvents> Events { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        //[ForeignKey("LastPaperEventId")]
        [ForeignKey("LastPaperEventId")]
        public virtual DocumentEvents LastPaperEvent { get; set; }
    }
}
