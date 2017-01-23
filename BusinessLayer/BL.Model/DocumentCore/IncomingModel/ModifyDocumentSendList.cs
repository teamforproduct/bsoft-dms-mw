using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Users;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для изменения записи плана работы над документом
    /// </summary>
    public class ModifyDocumentSendList : BaseModifyDocumentSendList
    {
        /// <summary>
        /// ИЗ записи плана
        /// </summary>
        public int Id { get; set; }
    }
}
