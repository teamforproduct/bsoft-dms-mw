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
        public List<int> Id { get; set; }
        /// <summary>
        /// ссылка на шаблон
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Должность
        /// </summary>
        public int? PositionId { get; set; }
        /// <summary>
        /// Фрагмент текста задачи
        /// </summary>
        public string Task { get; set; }
        

    }
}