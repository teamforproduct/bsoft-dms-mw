using System;
using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для списков рассылки в шаблонах документов
    /// </summary>
    public class FilterTemplateDocumentSendList
    {
        public List<int> Id { get; set; }
        public EnumSendTypes? SendType { get; set; }
        public int? TargetPositionId { get; set; }
        public int? Stage { get; set; }
        public string Task { get; set; }
        

    }
}