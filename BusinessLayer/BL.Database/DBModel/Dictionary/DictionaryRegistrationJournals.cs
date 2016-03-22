using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryRegistrationJournals
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        [MaxLength(2000)]
        public string Index { get; set; }
        [MaxLength(2000)]
        public string NumerationPrefixFormula { get; set; }
        [MaxLength(2000)]
        public string PrefixFormula { get; set; }
        [MaxLength(2000)]
        public string SuffixFormula { get; set; }
        [MaxLength(2000)]
        public string DirectionCodes { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DictionaryDepartments Department { get; set; }
    }
}
