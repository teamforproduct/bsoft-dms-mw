using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для копирования документа
    /// </summary>
    public class CopyDocument : CurrentPosition
    {
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int DocumentId { get; set; }
    }
}
