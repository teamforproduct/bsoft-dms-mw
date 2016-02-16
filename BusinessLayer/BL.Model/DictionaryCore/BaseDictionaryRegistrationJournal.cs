using System;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryRegistrationJournal
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

        public string DepartmentName { get; set; }

    }
}