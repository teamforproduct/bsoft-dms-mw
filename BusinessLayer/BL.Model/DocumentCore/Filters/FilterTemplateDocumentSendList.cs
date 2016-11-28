﻿using System;
using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    ///Фильтр для списков рассылки в шаблонах документов
    /// </summary>
    public class FilterTemplateDocumentSendList
    {
        /// <summary>
        /// ИД
        /// </summary>
        public List<int> IDs { get; set; }
        /// <summary>
        /// Ссылка на шаблон
        /// </summary>
        public int? TemplateId { get; set; }
        public EnumSendTypes? SendType { get; set; }
        public int? TargetPositionId { get; set; }
        public int? Stage { get; set; }
        public string Task { get; set; }
        

    }
}