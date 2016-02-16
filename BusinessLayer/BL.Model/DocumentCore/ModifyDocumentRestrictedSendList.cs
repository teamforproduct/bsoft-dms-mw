using BL.Model.Users;
using System;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД должности
        /// </summary>
        [Required]
        public Nullable<int> PositionId { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        [Required]
        public int AccessLevelId { get; set; }
    }
}
