using System;
using BL.Model.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для отправки сообщения членам рабочей группы
    /// </summary>
    public class SendMessage : CurrentPosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// массив ИД должностей, которым направляется сообщение
        /// </summary>
        [Required]
        public List<int> Positions { get; set; }
        /// <summary>
        /// Сообщение
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Добавлять ли в сообщение информацию о всех получателях сообщения
        /// </summary>
        [Required]
        public bool IsAddPositionsInfo { get; set; }
        /// <summary>
        /// Постоянное сообщение для всех участников рабочей группы(которые есть и будут)
        /// </summary>
        public bool IsForAllWorkGorupMember { get; set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate { get; set; }
    }
}
