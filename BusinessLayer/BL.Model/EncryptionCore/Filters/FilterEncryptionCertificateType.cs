using System;
using System.Collections.Generic;

namespace BL.Model.EncryptionCore.Filters
{
    /// <summary>
    /// Фильтр сертификатов
    /// </summary>
    public class FilterEncryptionCertificateType
    {
        /// <summary>
        /// Массив ИД типов
        /// </summary>
        public List<int> TypeId { get; set; }
        /// <summary>
        /// Отрывок названия
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Отрывок кода
        /// </summary>
        public string Code { get; set; }
    }
}
