﻿using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryPositionExecutor
    /// </summary>
    public class FilterDictionaryPositionExecutor : BaseFilterNameIsActive
    {

        /// <summary>
        /// Список должностей
        /// </summary>
        public List<int> PositionIDs { get; set; }

        /// <summary>
        /// Список сотрудников
        /// </summary>
        public List<int> AgentIDs { get; set; }

        /// <summary>
        /// Исключение сотрудников
        /// </summary>
        [IgnoreDataMember]
        public List<int> NotContainsAgentIDs { get; set; }

        /// <summary>
        /// Исключение должностей
        /// </summary>
        [IgnoreDataMember]
        public List<int> NotContainsPositionIDs { get; set; }

        /// <summary>
        /// Дата начала исполнения должности
        /// </summary>
        public DateTime? StartDate { get { return _StartDate; } set { _StartDate = value.ToUTC(); } }
        private DateTime? _StartDate;

        /// <summary>
        /// Дата окончания исполнения должности
        /// </summary>
        public DateTime? EndDate { get { return _EndDate; } set { _EndDate = value.ToUTC(); } }
        private DateTime? _EndDate;

        [IgnoreDataMember]
        public bool? ExistExecutorAgentInPositions { get; set; }

        /// <summary>
        /// Тип исполнения
        /// </summary>
        public List<EnumPositionExecutionTypes>  PositionExecutorTypeIDs { get; set; }

        /// <summary>
        /// Уровень доступа к документам: лично, референт, ио
        /// При создании документов всегда указывается уровень доступа для ио и референтов
        /// </summary>
        public List<EnumAccessLevels> AccessLevelIDs { get; set; }

    }
}
