using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    // Класс нативно отображает структуру таблицы DictionaryDocumentSubjects и ее связи (ParentDocumentSubject и ChildDocumentSubjects)
    // Этот класс сгененирован автоматически
    // Виртуальные свойства предписаны спецификацией Entity
    // Рассположение аттрибута [ForeignKey("ParentId")] также важно. Аттрибут должен рассполагаться над виртуальным свойством
    // Содержание класса должно строго соответствовать таблице. В противном случае из-за разности версий весь проект не стыкуется с базой. Все запросы перестают работать.
    public class DictionaryDocumentSubjects
    {
        public DictionaryDocumentSubjects()
        {
            this.ChildDocumentSubjects = new HashSet<DictionaryDocumentSubjects>();
        }

        public int Id { get; set; }
        [Index("IX_Name", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        public Nullable<int> ParentId { get; set; }
        [MaxLength(2000)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("ParentId")]
        public virtual DictionaryDocumentSubjects ParentDocumentSubject { get; set; }
        public virtual ICollection<DictionaryDocumentSubjects> ChildDocumentSubjects { get; set; }
    }
}
