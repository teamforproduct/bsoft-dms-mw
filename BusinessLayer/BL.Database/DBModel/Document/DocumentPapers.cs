using System;
using System.Collections.Generic;
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
            this.PaperEvents = new HashSet<DocumentPaperEvents>();
        }

        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public bool IsOriginal { get; set; }
        public bool IsCopy { get; set; }
        public int PageQuantity { get; set; }
        public int OrderNumber { get; set; }
        public int LastPaperEventId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("PaperId")]
        public virtual ICollection<DocumentPaperEvents> PaperEvents { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
    }
}
