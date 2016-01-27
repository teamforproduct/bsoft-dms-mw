using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryRegistrationJournals
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public string Index { get; set; }
        public string PrefixFormula { get; set; }
        public string SuffixFormula { get; set; }
        public string DirectionCodes { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }

        public virtual DictionaryDepartments Department { get; set; }
    }
}
