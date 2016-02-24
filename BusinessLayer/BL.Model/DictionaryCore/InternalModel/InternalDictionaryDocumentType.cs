using System;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryDocumentType : ModifyDictionaryDocumentType //TODO : LastChangeInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

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