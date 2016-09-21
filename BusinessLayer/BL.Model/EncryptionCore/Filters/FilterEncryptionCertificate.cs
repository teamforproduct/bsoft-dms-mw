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
        /// <summary>
        /// В информации о сертификате храняться данные с которых он валиден, если задан этот фильтр то соответственно отбираются активные или не активные сертификаты
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Только для админа, фильтрация по списку агентов
        /// </summary>
        public List<int> AgentId { get; set; }
    }
}
