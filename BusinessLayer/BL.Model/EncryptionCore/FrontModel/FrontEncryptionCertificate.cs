using BL.Model.EncryptionCore.InternalModel;
using BL.Model.Enums;
using System;

namespace BL.Model.EncryptionCore.FrontModel
{
    public class FrontEncryptionCertificate
    {
        public FrontEncryptionCertificate()
        {

        }
        public FrontEncryptionCertificate(InternalEncryptionCertificate model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.CreateDate = model.CreateDate;
            this.ValidFromDate = model.ValidFromDate;
            this.ValidToDate = model.ValidToDate;
            this.IsPublic = model.IsPublic;
            this.IsPrivate = model.IsPrivate;
            this.AgentId = model.AgentId;
            this.Extension = model.Extension;
            this.Type = model.Type;
        }
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
        /// Имя агента
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public EnumEncryptionCertificateTypes Type { get; set; }
        /// <summary>
        /// Имя типа
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Кто последний изменял
        /// </summary>
        public int? LastChangeUserId { get; set; }
        /// <summary>
        /// Дата последнего изменения
        /// </summary>
        public DateTime? LastChangeDate { get; set; }

        /// <summary>
        /// Cодержимое сертификата
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Расширение сертификата
        /// </summary>
        public string Extension { get; set; }
    }
}