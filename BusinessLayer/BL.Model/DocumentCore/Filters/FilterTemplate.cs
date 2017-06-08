using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для шаблонов документов
    /// </summary>
    public class FilterTemplate : BaseFilter
    {
        /// <summary>
        /// Массив ИД направлений документов
        /// </summary>
        public List<int> DocumentDirectionId { get; set; }
        /// <summary>
        /// Массив ИД типов документов
        /// </summary>
        public List<int> DocumentTypeId { get; set; }
        /// <summary>
        /// Отрывок тематики документа
        /// </summary>
        public string DocumentSubject { get; set; }
        /// <summary>
        /// Отрывок названия
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string NameExectly { get; set; }
        /// <summary>
        /// Отрывок краткого содержания
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Массив ИД журналов регистрации документов
        /// </summary>
        public List<int> RegistrationJournalId { get; set; }




    }
}