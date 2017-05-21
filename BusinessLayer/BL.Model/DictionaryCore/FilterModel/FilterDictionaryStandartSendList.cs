using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр для типовой рассылки
    /// </summary>
    public class FilterDictionaryStandartSendList : BaseFilterNameIsActive
    {
        /// <summary>
        /// Должность
        /// </summary>
        [IgnoreDataMember]
        public List<int> PositionIDs { get; set; }

        /// <summary>
        /// Отделы
        /// </summary>
        [IgnoreDataMember]
        public List<int> PositionDepartmentsIDs { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [IgnoreDataMember]
        public int? AgentId { get; set; }

    }
}
