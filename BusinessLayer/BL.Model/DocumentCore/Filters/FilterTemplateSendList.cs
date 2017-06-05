using System;
using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для списков рассылки в шаблонах документов
    /// </summary>
    public class FilterTemplateSendList
    {
        /// <summary>
        /// ИД
        /// </summary>
        public List<int> IDs { get; set; }
        /// <summary>
        /// ИД шаблона
        /// </summary>
        public int? TemplateId { get; set; }
        public EnumSendTypes? SendType { get; set; }
        public EnumStageTypes? StageType { get; set; }
        public int? Stage { get; set; }
        public string Task { get; set; }
        

    }
}