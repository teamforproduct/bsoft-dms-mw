using BL.Model.Users;
using System;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore
{
    /// <summary>
    /// Модель для модификации ограничительного списка
    /// </summary>
    public class ModifyDocumentRestrictedSendList
    {
        /// <summary>
        /// ИД записи
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД должности
        /// </summary>
        public Nullable<int> PositionId { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        public int AccessLevelId { get; set; }
    }
}
