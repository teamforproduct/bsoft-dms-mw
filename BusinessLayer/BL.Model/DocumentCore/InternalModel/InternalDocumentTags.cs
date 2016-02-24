using BL.Model.DocumentCore.IncomingModel;
using System;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentTags : ModifyDocumentTags //TODO Сергей: LastChangeInfo?
    {
        /// <summary>
        /// ИД. пользователя, изменившего запись
        /// </summary>
        public int LastChangeUserId { get; set; }

        /// <summary>
        /// Дата последнего изменения записи
        /// </summary>
        public DateTime LastChangeDate { get; set; }
    }
}
