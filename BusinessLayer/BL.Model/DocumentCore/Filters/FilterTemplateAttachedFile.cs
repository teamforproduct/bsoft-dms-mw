using System;
using System.Collections.Generic;
using BL.Model.Enums;
using BL.Model.Common;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для задач в шаблонах документов
    /// </summary>
    public class FilterTemplateAttachedFile : BaseFilter
    {
        /// <summary>
        /// Ссылка на шаблон
        /// </summary>
        public int? TemplateId { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Расширение файла
        /// </summary>
        public string Extention { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>
        public string NameExactly { get; set; }

        /// <summary>
        /// Расширение файла
        /// </summary>
        public string ExtentionExactly { get; set; }

    }
}