using System;
using BL.Model.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для отправки сообщения членам рабочей группы
    /// </summary>
    public class SendMessage : AddNote
    {
        /// <summary>
        /// группы получателей сообщения
        /// </summary>
        public List<AccessGroup> TargetAccessGroups { get; set; }

    }
}
