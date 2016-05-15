using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    public class ControlOff
    {
        /// <summary>
        /// ИД события, породившего контроль
        /// </summary>
        [Required]
        public int EventId { get; set; }
        /// <summary>
        /// ИД типа результата
        /// </summary>
        public int ResultTypeId { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Каскадное закрытие контроля
        /// </summary>
        public bool IsCascade { get; set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate { get; set; }
    }
}
