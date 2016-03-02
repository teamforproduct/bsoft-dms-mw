using System;
using System.Collections.Generic;
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
        public string Code { get; set; }
        public string Description { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<CustomDictionaries> CustomDictionaries { get; set; }
    }
}
