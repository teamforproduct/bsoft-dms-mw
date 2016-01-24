using System;
using System.Collections.Generic;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryAgents
    {
        public DictionaryAgents()
        {
            this.Positions = new HashSet<DictionaryPositions>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<DictionaryPositions> Positions { get; set; }
    }
}
