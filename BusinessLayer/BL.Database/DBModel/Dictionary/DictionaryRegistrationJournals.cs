using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    // Класс нативно отображает структуру таблицы [DictionaryRegistrationJournals] и ее связи (Department)
    // Этот класс сгененирован автоматически
    // Виртуальные свойства предписаны спецификацией Entity
    // Рассположение аттрибута [ForeignKey("DepartmentId")] также важно. Аттрибут должен рассполагаться над виртуальным свойством
    // Содержание класса должно строго соответствовать таблице. В противном случае из-за разности версий весь проект не стыкуется с базой. Все запросы перестают работать.
    public class DictionaryRegistrationJournals
    {
        public int Id { get; set; }
        [Index("IX_Name", 4, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(200)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        [Index("IX_Name", 2, IsUnique = true)]
        [Index("IX_DepartmentId", 1)]
        public int DepartmentId { get; set; }
        [Index("IX_Name", 3, IsUnique = true)]
        [MaxLength(200)]
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
