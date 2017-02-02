using System;
using System.Collections.Generic;
using BL.Model.Enums;
using BL.Model.Common;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для задач в шаблонах документов
    /// </summary>
    public class FilterTemplateDocumentPaper : BaseFilter
    {
        /// <summary>
        /// ссылка на шаблон
        /// </summary>
        public int? TemplateId { get; set; }
        

    }
}