using BL.Model.Enums;
using BL.Model.Users;
using System;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace BL.Model.EncryptionCore.IncomingModel
{
    /// <summary>
    /// Добавления файла сертификата
    /// </summary>
    public class AddEncryptionCertificate
    {
        /// <summary>
        /// Название сертификата
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Действует с даты
        /// </summary>
        public DateTime? NotBefore { get; set; }
        /// <summary>
        /// Действует по дату
        /// </summary>
        public DateTime? NotAfter { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public HttpPostedFile PostedFileData { get; set; }
    }
}
