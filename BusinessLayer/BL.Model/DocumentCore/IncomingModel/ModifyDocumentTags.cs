using BL.Model.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для перезаписи списка тегов документа
    /// </summary>
    public class ModifyDocumentTags: CurrentPosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Список ИД тегов
        /// </summary>
        [Required]
        public List<int> Tags { get; set; }
    }
}
