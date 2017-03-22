using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using BL.Model.Users;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления плана работы над документом по стандартному списку
    /// </summary>
    public class ModifyDocumentSendListByStandartSendList: CurrentPosition
    {
        /// <summary>
        /// ИД стандартного списка
        /// </summary>
        [Required]
        public int StandartSendListId { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Не запонять!!!
        /// Признак первоначальный пункт
        /// </summary>
        [IgnoreDataMember]
        public bool IsInitial { get; set; }
    }
}
