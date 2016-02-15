using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    /// <summary>
    /// Модель для добавление документа по шаблону
    /// </summary>
    public class AddDocumentByTemplateDocument : CurrentPosition
    {
        /// <summary>
        /// ИД шаблона документа
        /// </summary>
        [Required]
        public int TemplateDocumentId { get; set; }
        /// <summary>
        /// ИД документа, с которым нужно установить связь (если необходимо)
        /// </summary>
        public int? ParentDocumentId { get; set; }
    }
}
