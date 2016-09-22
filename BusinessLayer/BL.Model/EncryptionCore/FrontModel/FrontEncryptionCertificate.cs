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
            this.NotBefore = model.NotBefore;
            this.NotAfter = model.NotAfter;
            this.AgentId = model.AgentId;
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
        public DateTime? NotBefore { get; set; }
        /// <summary>
        /// Действует по дату
        /// </summary>
        public DateTime? NotAfter { get; set; }

        /// <summary>
        /// ИД Agent
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// Имя Agent
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// Можно ли запомнить пароль к сертификату
        /// </summary>
        public bool IsRememberPassword { get; set; }

        /// <summary>
        /// Кто последний изменял
        /// </summary>
        public int? LastChangeUserId { get; set; }
        /// <summary>
        /// Дата последнего изменения
        /// </summary>
        public DateTime? LastChangeDate { get; set; }
    }
}