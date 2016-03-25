using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentPaperLists
    {
        public DocumentPaperLists()
        {
            this.PaperEvents = new HashSet<DocumentPaperEvents>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<DocumentPaperEvents> PaperEvents { get; set; }
    }
}
