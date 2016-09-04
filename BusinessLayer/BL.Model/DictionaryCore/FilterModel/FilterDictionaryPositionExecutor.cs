using BL.Model.Common;
using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryPositionExecutor
    /// </summary>
    public class FilterDictionaryPositionExecutor : DictionaryBaseFilterParameters
    {

        /// <summary>
        /// Список должностей
        /// </summary>
        public List<int> PositionIDs { get; set; }

        /// <summary>
        /// Список агентов
        /// </summary>
        public List<int> AgentIDs { get; set; }

        /// <summary>
        /// Дата начала исполнения должностных обязанностей
        /// </summary>
        public Period Period { get; set; }

        /// <summary>
        /// Тип исполнения: 
        /// 1 - Назначен на должность;
        /// 2 - Исполяет обязанности;
        /// 3 - Является референтом;
        /// </summary>
        public List<int>  PositionExecutorTypeIDs { get; set; }

        /// <summary>
        /// Уровень доступа к документам: лично, референт, ио
        /// При создании документов всегда указывается уровень доступа для ио и референтов
        /// </summary>
        public List<int> AccessLevelIDs { get; set; }

    }
}
