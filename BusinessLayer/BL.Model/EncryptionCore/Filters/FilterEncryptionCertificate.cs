using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.EncryptionCore.Filters
{
    /// <summary>
    /// Фильтр сертификатов
    /// </summary>
    public class FilterEncryptionCertificate
    {
        /// <summary>
        /// Массив ИД сертификатов
        /// </summary>
        public List<int> CertificateId { get; set; }

        /// <summary>
        /// Массив типов
        /// </summary>
        public List<EnumEncryptionCertificateTypes> TypeId { get; set; }

        /// <summary>
        /// Отрывок названия
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отрывок расширение сертификата
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Дата "с" для отбора по дате действия
        /// </summary>
        public DateTime? ValidFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате действия
        /// </summary>
        public DateTime? ValidToDate { get; set; }

        /// <summary>
        /// Дата "с" для отбора по дате внесения сертификата в систему
        /// </summary>
        public DateTime? CreateFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате внесения сертификата в систему
        /// </summary>
        public DateTime? CreateToDate { get; set; }

        /// <summary>
        /// Признак публичного ключа
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// Признак приватного ключа
        /// </summary>
        public bool? IsPrivate { get; set; }
    }
}
