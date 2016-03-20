using System;
using BL.Model.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для отправки сообщения членам рабочей группы
    /// </summary>
    public class SendMessage : AddNote
    {
        /// <summary>
        /// массив ИД должностей, которым направляется сообщение
        /// </summary>
        [Required]
        public List<int> Positions { get; set; }
        /// <summary>
        /// Добавлять ли в сообщение информацию о всех получателях сообщения
        /// </summary>
        [Required]
        public bool IsAddPositionsInfo { get; set; }
    }
}
