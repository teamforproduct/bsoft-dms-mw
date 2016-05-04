using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель изменения параметров контроля для исполнителя
    /// </summary>
    public class ControlTargetChange 
    {
        /// <summary>
        /// ИД события, породившего контроль
        /// </summary>
        [Required]
        public int EventId { get; set; }
        /// <summary>
        /// Комментарий исполнителя
        /// </summary>
        public string TargetDescription { get; set; }
        /// <summary>
        /// Дата включения режима постоянное внимание исполнителя
        /// </summary>
        public DateTime? TargetAttentionDate { get; set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate { get; set; }
    }
}
