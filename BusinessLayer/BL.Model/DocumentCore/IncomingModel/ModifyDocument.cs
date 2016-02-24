using System;
using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;

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
        /// ИД Тематики документа
        /// </summary>
        public int? DocumentSubjectId { get; set; }
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
        public DateTime? SenderDate { get; set; }
        /// <summary>
        /// Кому адресован документ
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        [Required]
        public EnumDocumentAccesses AccessLevel { get; set; }
    }
}
