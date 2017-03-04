using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для шаблонов документов
    /// </summary>
    public class FilterTemplateDocument : BaseFilter
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
        /// Массив ИД тематик документов
        /// </summary>
        public List<int> DocumentSubjectId { get; set; }
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