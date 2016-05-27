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
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания исполнения должностных обязанностей
        /// </summary>
        public DateTime EndDate { get; set; }

    }
}
