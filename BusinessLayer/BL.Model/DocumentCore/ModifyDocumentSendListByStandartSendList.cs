using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    /// <summary>
    /// Модель для добавления плана работы над документом по стандартному списку
    /// </summary>
    public class ModifyDocumentSendListByStandartSendList
    {
        /// <summary>
        /// ИД стандартного списка
        /// </summary>
        public int StandartSendListId { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
    }
}
