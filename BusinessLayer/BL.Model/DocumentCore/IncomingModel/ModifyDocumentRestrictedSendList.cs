using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для модификации ограничительного списка
    /// </summary>
    public class ModifyDocumentRestrictedSendList
    {
        /// <summary>
        /// ИД записи
        /// </summary>
       // [IgnoreDataMember]
       // public int Id { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД должности
        /// </summary>
        [Required]
        public int? PositionId { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        [Required]
        public EnumAccessLevels AccessLevel { get; set; }
    }
}
