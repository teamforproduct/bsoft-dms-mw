using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для связывания документов
    /// </summary>
    public class AddDocumentLink
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД родительского документа
        /// </summary>
        [Required]
        public int ParentDocumentId { get; set; }
        /// <summary>
        /// ИД типа связи
        /// </summary>
        [Required]
        public int LinkTypeId { get; set; }       

    }
}
