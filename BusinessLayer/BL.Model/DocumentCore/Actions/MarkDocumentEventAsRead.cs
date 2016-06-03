using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для отметки пользователем ивентов документа как прочитанные
    /// </summary>
    public class MarkDocumentEventAsRead
    {
        [Required]
        public List<int> EventIds { get; set; }       

    }
}
