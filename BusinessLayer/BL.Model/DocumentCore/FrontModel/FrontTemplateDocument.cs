using System;
using System.Collections.Generic;
using BL.Model.Enums;
using BL.Model.SystemCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Модель шаблона документа для фронта
    /// </summary>
    public class FrontTemplateDocument : ModifyTemplateDocument
    {
        
        /// <summary>
        /// Название гаправления
        /// </summary>
        public string DocumentDirectionName { get; set; }
        /// <summary>
        /// Название типа документа
        /// </summary>
        public string DocumentTypeName { get; set; }
        
        /// <summary>
        /// Название тематики документа
        /// </summary>
        public string DocumentSubjectName { get; set; }

        /// <summary>
        /// Название журнала регистрации
        /// </summary>
        public string RegistrationJournalName { get; set; }
        /// <summary>
        /// Название Контрагента, от которого получен документ - только для входящих
        /// </summary>
        public string SenderAgentName { get; set; }
        
        /// <summary>
        /// Название Контактное лицо в организации
        /// </summary>
        public string SenderAgentPersonName { get; set; }
        
        /// <summary>
        /// Есть ли документы, созданные по шаблону
        /// </summary>
        public bool? IsUsedInDocument { get; set; }
        
    }
}
