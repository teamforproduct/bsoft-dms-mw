using System;
using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для задач в шаблонах документов
    /// </summary>
    public class FilterTemplateDocumentTask
    {
        /// <summary>
        /// ИД
        /// </summary>
        public List<int> IDs { get; set; }
        /// <summary>
        /// Ссылка на шаблон
        /// </summary>
        public int? TemplateId { get; set; }
        /// <summary>
        /// Фрагмент текста задачи
        /// </summary>
        public string Task { get; set; }
        

    }
}