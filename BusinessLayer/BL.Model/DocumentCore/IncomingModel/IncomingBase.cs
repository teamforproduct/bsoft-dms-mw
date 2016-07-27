using System;
using System.ComponentModel.DataAnnotations;
using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.SystemCore.IncomingModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using BL.Model.DocumentCore.Filters;
using BL.Model.SystemCore;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для Post GetList
    /// </summary>
    public class IncomingBase
    {
        /// <summary>
        /// Модель фильтра
        /// </summary>
        public FilterBase Filter { get; set; }
        /// <summary>
        /// Модель Paging
        /// </summary>
        public UIPaging Paging { get; set; }
    }
}
