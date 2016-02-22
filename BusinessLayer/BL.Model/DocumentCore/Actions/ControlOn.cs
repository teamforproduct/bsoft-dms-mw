using BL.Model.Users;
using System;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для взятия документа на контроль
    /// </summary>
    public class ControlOn : CurrentPosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Задача
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Контрольный срок
        /// </summary>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Срок включения режима постоянное внимание
        /// </summary>
        public DateTime? AttentionDate { get; set; }
    }
}
