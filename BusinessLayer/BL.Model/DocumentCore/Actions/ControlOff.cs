using BL.Model.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для снятия контроля
    /// </summary>
    public class ControlOff: SendEventMessage
    {
        /// <summary>
        /// ИД типа результата
        /// </summary>
        public int? ResultTypeId { get; set; }
        /// <summary>
        /// Каскадное закрытие контроля
        /// </summary>
        public bool IsCascade { get; set; }
    }
}
