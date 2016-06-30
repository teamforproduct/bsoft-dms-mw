using System;
using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для задач в шаблонах документов
    /// </summary>
    public class FilterTemplateAttachedFile
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int? FileId { get; set; }
        /// <summary>
        /// Фрагмент имени файла
        /// </summary>
        public string Name { get; set; }
        

    }
}