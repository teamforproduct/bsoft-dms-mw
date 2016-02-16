using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    /// <summary>
    /// Модель для добавления ограничительного списка по стандартному списку
    /// </summary>
    public class ModifyDocumentRestrictedSendListByStandartSendList
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
    }
}
