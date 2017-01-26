using BL.Model.Enums;
using BL.Model.Extensions;
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
        public DateTime? NotBefore { get { return _NotBefore; } set { _NotBefore = value.ToUTC(); } }
        private DateTime? _NotBefore;
        /// <summary>
        /// Действует по дату
        /// </summary>
        public DateTime? NotAfter { get { return _NotAfter; } set { _NotAfter = value.ToUTC(); } }
        private DateTime? _NotAfter;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ID Агента. Используется только админом
        /// </summary>
        [IgnoreDataMember]
        public int? AgentId { get; set; }

        /// <summary>
        /// Можно ли запомнить пароль к сертификату
        /// </summary>
        public bool IsRememberPassword { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public HttpPostedFile PostedFileData { get; set; }
    }
}
