using System;
using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для задач в шаблонах документов
    /// </summary>
    public class FilterTemplateDocumentPaper
    {
        /// <summary>
        /// ИД
        /// </summary>
        public List<int> IDs { get; set; }
        /// <summary>
        /// ссылка на шаблон
        /// </summary>
        public int? TemplateId { get; set; }
        

    }
}