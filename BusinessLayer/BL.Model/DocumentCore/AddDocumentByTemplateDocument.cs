using BL.Model.Users;
using System;
using System.Collections.Generic;
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
        public int TemplateDocumentId { get; set; }
    }
}
