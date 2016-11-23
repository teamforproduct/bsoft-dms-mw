using System;
using System.Collections.Generic;
using BL.Model.Enums;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для шаблонов документов
    /// </summary>
    public class FilterTemplateDocument
    {
        /// <summary>
        /// Массив ИД шаблонов документов
        /// </summary>
        public List<int> TemplateDocumentId { get; set; }
        /// <summary>
        /// Массив ИД документов полученного из полнотекстового поиска
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public List<int> FullTextSearchDocumentId { get; set; }
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
        /// Отрывок краткого содержания
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Массив ИД журналов регистрации документов
        /// </summary>
        public List<int> RegistrationJournalId { get; set; }




    }
}