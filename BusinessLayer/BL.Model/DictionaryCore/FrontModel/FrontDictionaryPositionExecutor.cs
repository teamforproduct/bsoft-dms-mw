using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;
using System;
using BL.Model.Enums;
using System.Runtime.Serialization;
using BL.Model.AdminCore.FrontModel;
using BL.Model.Extensions;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Исполнители". 
    /// </summary>
    public class FrontDictionaryPositionExecutor: ModifyPositionExecutor
    {

        /// <summary>
        /// Агент
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionFullName { get; set; }

        /// <summary>
        /// Тип исполнителя
        /// </summary>
        public string PositionExecutorTypeName { get; set; }

        /// <summary>
        /// Краткое название (как приставка к названию должности) типа исполнителя
        /// </summary>
        public string PositionExecutorTypeSuffix { get; set; }

        /// <summary>
        /// Департамент
        /// </summary>
        public string DepartmentIndex { get; set; }

        /// <summary>
        /// Департамент
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Уровень доступа: лично, референт, ио
        /// </summary>
        public string AccessLevelName { get; set; }


    }
}