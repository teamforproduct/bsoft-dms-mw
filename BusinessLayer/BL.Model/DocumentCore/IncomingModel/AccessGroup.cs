using BL.Model.Enums;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// группы получателей
    /// </summary>
    public class AccessGroup
    {
        /// <summary>
        /// тип доступа к событию
        /// </summary>
        public EnumEventAccessTypes AccessType { get; set; }   // получатель, копия, досылка
        /// <summary>
        /// тип группы
        /// </summary>
        public EnumEventAccessGroupTypes AccessGroupType { get; set; } //тип группы, в т.ч. РГ по доку
        /// <summary>
        /// ИД
        /// </summary>
        public int? RecordId { get; set; }
    }
}
