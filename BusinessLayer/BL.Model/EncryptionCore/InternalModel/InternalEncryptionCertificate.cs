using BL.Model.Common;
using BL.Model.Enums;
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
        /// Thumbprint
        /// </summary>
        public string Thumbprint { get; set; }


        /// <summary>
        /// Дата внесения сертификата в систему
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Действует с даты
        /// </summary>
        public DateTime? NotBefore { get; set; }
        /// <summary>
        /// Действует по дату
        /// </summary>
        public DateTime? NotAfter { get; set; }
        /// <summary>
        /// ИД Position
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// Cодержимое сертификата Zip
        /// </summary>
        public byte[] CertificateZip { get; set; }

        /// <summary>
        /// Cодержимое сертификата
        /// </summary>
        public byte[] Certificate { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}