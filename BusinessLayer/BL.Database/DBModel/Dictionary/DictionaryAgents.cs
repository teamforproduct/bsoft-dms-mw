using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public partial class DictionaryAgents
    {
        public DictionaryAgents()
        {
            this.Positions = new HashSet<DictionaryPositions>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }

        public virtual ICollection<DictionaryPositions> Positions { get; set; }
    }
}
