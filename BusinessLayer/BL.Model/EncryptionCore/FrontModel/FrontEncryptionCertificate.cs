﻿using BL.Model.EncryptionCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Extensions;
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
        public DateTime CreateDate { get { return _CreateDate; } set { _CreateDate = value.ToUTC(); } }
        private DateTime _CreateDate;

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
        public DateTime? LastChangeDate { get { return _LastChangeDate; } set { _LastChangeDate = value.ToUTC(); } }
        private DateTime? _LastChangeDate;
    }
}