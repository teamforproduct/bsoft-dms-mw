﻿using BL.Model.Enums;
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
        /// Тип
        /// </summary>
        public EnumEncryptionCertificateTypes Type { get; set; }

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
        public HttpPostedFile PostedFileData { get; set; }
        /// <summary>
        /// Данные файла
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public byte[] Certificate { get; set; }
    }
}