using BL.Database.DBModel.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public partial class DictionaryTags
    {
        public DictionaryTags()
        {
            this.Documents = new HashSet<DocumentTags>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> PositionId { get; set; }
        public string Color { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }

        public virtual ICollection<DocumentTags> Documents { get; set; }
    }
}
