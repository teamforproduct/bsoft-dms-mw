using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentPaperLists
    {
        public DocumentPaperLists()
        {
            this.Events = new HashSet<DocumentEvents>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<DocumentEvents> Events { get; set; }
    }
}
