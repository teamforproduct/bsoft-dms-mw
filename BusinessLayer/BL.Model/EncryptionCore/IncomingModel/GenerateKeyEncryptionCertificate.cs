using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.EncryptionCore.IncomingModel
{
    /// <summary>
    /// Генерация сертификата
    /// </summary>
    public class GenerateKeyEncryptionCertificate
    {
        /// <summary>
        /// Название сертификата
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Действует с даты
        /// </summary>
        public DateTime? ValidFromDate { get; set; }
        /// <summary>
        /// Действует по дату
        /// </summary>
        public DateTime? ValidToDate { get; set; }
        /// <summary>
        /// Признак публичного ключа
        /// </summary>
        public bool IsPublic { get; set; }
        /// <summary>
        /// Признак приватного ключа
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public byte[] Certificate { get; set; }
    }
}
