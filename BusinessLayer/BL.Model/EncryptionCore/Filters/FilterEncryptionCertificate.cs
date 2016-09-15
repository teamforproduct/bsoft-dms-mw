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
        /// Отрывок названия
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата "с" для отбора по дате действия
        /// </summary>
        public DateTime? NotBefore { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате действия
        /// </summary>
        public DateTime? NotAfter { get; set; }

        /// <summary>
        /// Дата "с" для отбора по дате внесения сертификата в систему
        /// </summary>
        public DateTime? CreateFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате внесения сертификата в систему
        /// </summary>
        public DateTime? CreateToDate { get; set; }
        public bool? IsActive { get; set; }

    }
}
