using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.Extensions;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для модификации документа
    /// </summary>
    public class ModifyDocument
    {
        /// <summary>
        /// ИД Документа
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Тематика документа
        /// </summary>
        public string DocumentSubject { get; set; }
        /// <summary>
        /// Краткое содержание
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// ИД организации, обязательное для внешних документов
        /// </summary>
        public int? SenderAgentId { get; set; }
        /// <summary>
        /// ИД контакта, обязательное для внешних документов
        /// </summary>
        public int? SenderAgentPersonId { get; set; }
        /// <summary>
        /// Входящий номер документа
        /// </summary>
        public string SenderNumber { get; set; }
        /// <summary>
        /// Дата входящего документа
        /// </summary>
        public DateTime? SenderDate { get { return _SenderDate; } set { _SenderDate = value.ToUTC(); } }
        private DateTime? _SenderDate;
        /// <summary>
        /// Кому адресован документ
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        [Required]
        public  EnumAccessLevels AccessLevel { get; set; }

        public string ServerPath { get; set; }
        public IEnumerable<ModifyPropertyValue> Properties { get; set; }
    }
}
