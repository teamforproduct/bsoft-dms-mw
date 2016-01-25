using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionarySendTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsImpotant { get; set; }
        public int SubordinationTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }

        public virtual DictionarySubordinationTypes SubordinationType { get; set; }
    }
}
