using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public partial class CustomDictionaryTypes
    {
        public CustomDictionaryTypes()
        {
            this.CustomDictionaries = new HashSet<CustomDictionaries>();
        }

        public int Id { get; set; }
        [MaxLength(2000)]
        [Index("IX_Code", 1, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<CustomDictionaries> CustomDictionaries { get; set; }
    }
}
