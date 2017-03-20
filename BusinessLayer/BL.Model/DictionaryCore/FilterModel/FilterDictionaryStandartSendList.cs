using BL.Model.Common;
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
        public int? PositionID { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [IgnoreDataMember]
        public int? AgentId { get; set; }

    }
}
