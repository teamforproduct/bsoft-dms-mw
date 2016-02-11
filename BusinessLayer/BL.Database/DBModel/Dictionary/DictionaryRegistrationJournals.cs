using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryRegistrationJournals
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public string Index { get; set; }
        public string NumerationPrefixFormula { get; set; }
        public string PrefixFormula { get; set; }
        public string SuffixFormula { get; set; }
        public string DirectionCodes { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DictionaryDepartments Department { get; set; }
    }
}
