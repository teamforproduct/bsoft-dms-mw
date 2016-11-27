using BL.Model.Enums;
using BL.Model.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель подписания
    /// </summary>
    public class SelfAffixSigning
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime? EventDate
        {
            get { return _eventDate; }
            set { _eventDate = value.HasValue ? value.Value.ToUniversalTime() : value; }
        }
        private DateTime? _eventDate { get; set; }
        /// <summary>
        /// Текст визы
        /// </summary>
        public string VisaText { get; set; }

        /// <summary>
        /// Тип подписи
        /// </summary>
        public EnumSigningTypes SigningType { get; set; }

        /// <summary>
        /// ИД должности, от которой будет выполнятся действие
        /// </summary>
        public int? CurrentPositionId { get; set; }

        public int? CertificateId { get; set; }
        public string CertificatePassword { get; set; }
    }
}
