using BL.Model.Common;
using System;
using System.Web;

namespace BL.Model.EncryptionCore.InternalModel
{
    public class InternalEncryptionCertificate: LastChangeInfo
    {
        /// <summary>
        /// ИД сертификата
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Названия
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Дата внесения сертификата в систему
        /// </summary>
        public DateTime CreateDate { get; set; }

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
        /// ИД агента
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// Cодержимое сертификата
        /// </summary>
        public byte[] Certificate { get; set; }

        public HttpPostedFile PostedFileData { get; set; }

        /// <summary>
        /// Расширение сертификата
        /// </summary>
        public string Extension { get; set; }

    }
}