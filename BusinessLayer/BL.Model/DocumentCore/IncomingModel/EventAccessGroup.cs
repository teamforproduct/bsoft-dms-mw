using BL.Model.Enums;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// группы получателей
    /// </summary>
    public class EventAccessGroup
    {
        /// <summary>
        /// тип доступа к событию
        /// </summary>
        public EnumEventAccessTypes AccessType { get; set; }   // получатель, копия, досылка
        /// <summary>
        /// тип группы
        /// </summary>
        public EnumEventAccessGroupsTypes AccessGroupsType { get; set; } //тип группы, в т.ч. РГ по доку
        /// <summary>
        /// ИД
        /// </summary>
        public int? RecordId { get; set; }
    }
}
