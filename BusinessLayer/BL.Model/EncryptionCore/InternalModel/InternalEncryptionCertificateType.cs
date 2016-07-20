using BL.Model.Common;
using System;
using System.Web;

namespace BL.Model.EncryptionCore.InternalModel
{
    public class InternalEncryptionCertificateType : LastChangeInfo
    {
        /// <summary>
        /// ИД типа сертификата
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Названия
        /// </summary>
        public string Name { get; set; }

    }
}