using BL.Database.DBModel.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Index("IX_PositionName", 3, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(900)]
        [Index("IX_PositionName", 2, IsUnique = true)]
        public string Name { get; set; }
        [Index("IX_PositionName", 1, IsUnique = true)]
        public Nullable<int> PositionId { get; set; }
        [MaxLength(2000)]
        public string Color { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }

        public virtual ICollection<DocumentTags> Documents { get; set; }
    }
}
